using BuildingBlocks.Persistence;
using BuildingBlocks.Results;
using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.X509;
using iText.Forms.Form.Element;
using iText.IO.Image;
using iText.Kernel.Crypto;
using iText.Kernel.Pdf;
using iText.Signatures;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Pkcs;
using PdfService.Application.Common.Interfaces;
using PdfService.Application.Common.Models;
using System.Net;
using DomainCertificate = PdfService.Domain.Entities.Certificate;

namespace PdfService.Infrastructure.Services;

public sealed class PdfSigningService : IPdfSigningService
{
    private const string SignatureFieldName = "NeuroVisionSignature";
    private const string SignatureImageFileName = "signature.png";

    private readonly IRepository<DomainCertificate, Guid> _certificateRepository;
    private readonly ICertificateStorage _storage;
    private readonly ICertificatePasswordProtector _passwordProtector;
    private readonly ILogger<PdfSigningService> _logger;

    public PdfSigningService(
        IRepository<DomainCertificate, Guid> certificateRepository,
        ICertificateStorage storage,
        ICertificatePasswordProtector passwordProtector,
        ILogger<PdfSigningService> logger)
    {
        _certificateRepository = certificateRepository;
        _storage = storage;
        _passwordProtector = passwordProtector;
        _logger = logger;
    }

    public SignaturePosition ResolvePosition(byte[] pdfBytes, PdfService.Domain.Entities.PdfTemplate template) =>
        SignaturePlacementHelper.ResolveFromPdf(pdfBytes, template);

    public async Task<Result<byte[]>> SignPdfAsync(
        byte[] pdfBytes,
        Guid certificateId,
        SignaturePosition position,
        string? reason,
        string? location,
        CancellationToken cancellationToken = default)
    {
        if (pdfBytes is null || pdfBytes.Length == 0)
        {
            return Result<byte[]>.Fail(
                "The PDF file is empty or was not provided.",
                HttpStatusCode.BadRequest);
        }

        var certificateRecord = await _certificateRepository.GetByIdAsync(certificateId, cancellationToken);
        if (certificateRecord is null)
        {
            _logger.LogWarning("Certificate not found. CertificateId={CertificateId}", certificateId);
            return Result<byte[]>.Fail("Certificate not found.", HttpStatusCode.NotFound);
        }

        if (certificateRecord.IsExpired())
        {
            _logger.LogWarning(
                "Attempted to sign with expired certificate. CertificateId={CertificateId}",
                certificateId);

            return Result<byte[]>.Fail(
                "Cannot sign with an expired certificate.",
                HttpStatusCode.BadRequest);
        }

        byte[] pfxBytes;
        try
        {
            pfxBytes = await _storage.ReadAsync(certificateRecord.FilePath, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to read certificate file from storage. CertificateId={CertificateId}",
                certificateId);

            return Result<byte[]>.Fail(
                "Failed to read the certificate file.",
                HttpStatusCode.InternalServerError);
        }

        string password;
        try
        {
            password = _passwordProtector.Unprotect(certificateRecord.ProtectedPassword);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to decrypt stored certificate password. CertificateId={CertificateId}",
                certificateId);

            Array.Clear(pfxBytes, 0, pfxBytes.Length);
            return Result<byte[]>.Fail(
                "Failed to unlock the certificate.",
                HttpStatusCode.InternalServerError);
        }

        char[]? passwordChars = null;

        try
        {
            passwordChars = password.ToCharArray();
            var pkcs12Store = new Pkcs12StoreBuilder().Build();

            using var pfxStream = new MemoryStream(pfxBytes);
            try
            {
                pkcs12Store.Load(pfxStream, passwordChars);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to unlock stored certificate — password may be out of sync. CertificateId={CertificateId}",
                    certificateId);

                return Result<byte[]>.Fail(
                    "Unable to unlock the certificate for signing.",
                    HttpStatusCode.InternalServerError);
            }

            var alias = pkcs12Store.Aliases
                .Cast<string>()
                .FirstOrDefault(a => pkcs12Store.IsKeyEntry(a));

            if (alias is null)
            {
                _logger.LogError(
                    "No private key found in certificate file. CertificateId={CertificateId}",
                    certificateId);

                return Result<byte[]>.Fail(
                    "No private key found in the certificate file.",
                    HttpStatusCode.InternalServerError);
            }

            var privateKeyEntry = pkcs12Store.GetKey(alias);
            var rawCertificateChain = pkcs12Store.GetCertificateChain(alias)
                .Select(entry => entry.Certificate)
                .ToArray();

            if (rawCertificateChain.Length == 0)
            {
                _logger.LogError(
                    "Certificate chain is empty. CertificateId={CertificateId}",
                    certificateId);

                return Result<byte[]>.Fail(
                    "The certificate file does not contain a valid certificate chain.",
                    HttpStatusCode.InternalServerError);
            }

            var wrappedPrivateKey = new PrivateKeyBC(privateKeyEntry.Key);
            var wrappedCertificateChain = rawCertificateChain
                .Select(cert => new X509CertificateBC(cert))
                .ToArray();

            try
            {
                using var inputStream = new MemoryStream(pdfBytes);
                using var outputStream = new MemoryStream();

                var reader = new PdfReader(inputStream);
                var signer = new PdfSigner(
                    reader,
                    outputStream,
                    new StampingProperties().UseAppendMode());

                var pageRect = SignaturePlacementHelper.ToPageRect(position);

                _logger.LogInformation(
                    "Adding digital signature Page:{Page} X:{X} Y:{Y} Width:{Width} Height:{Height}",
                    position.Page,
                    position.X,
                    position.Y,
                    position.Width,
                    position.Height);

                var signerProperties = new SignerProperties()
                    .SetFieldName(SignatureFieldName)
                    .SetReason(reason ?? string.Empty)
                    .SetLocation(location ?? string.Empty)
                    .SetPageNumber(position.Page)
                    .SetPageRect(pageRect);

                await ApplySignatureAppearanceAsync(signerProperties, cancellationToken);
                signer.SetSignerProperties(signerProperties);

                var privateKeySignature = new PrivateKeySignature(
                    wrappedPrivateKey,
                    DigestAlgorithms.SHA256);

                signer.SignDetached(
                    privateKeySignature,
                    wrappedCertificateChain,
                    null,
                    null,
                    null,
                    0,
                    PdfSigner.CryptoStandard.CMS);

                _logger.LogInformation(
                    "PDF signed successfully. CertificateId={CertificateId}",
                    certificateId);

                return Result<byte[]>.Ok(outputStream.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to sign PDF. CertificateId={CertificateId}",
                    certificateId);

                return Result<byte[]>.Fail(
                    "Failed to sign the PDF file.",
                    HttpStatusCode.InternalServerError);
            }
        }
        finally
        {
            if (passwordChars is not null)
                Array.Clear(passwordChars, 0, passwordChars.Length);

            Array.Clear(pfxBytes, 0, pfxBytes.Length);
        }
    }

    private async Task ApplySignatureAppearanceAsync(
        SignerProperties signerProperties,
        CancellationToken cancellationToken)
    {
        var imageBytes = await _storage.TryReadSignatureImageAsync(SignatureImageFileName, cancellationToken);
        if (imageBytes is not { Length: > 0 })
        {
            _logger.LogWarning("Signature image not found. Digital signature will have no visual graphic.");
            return;
        }

        try
        {
            var imageData = ImageDataFactory.Create(imageBytes);
            var appearance = new SignatureFieldAppearance(SignatureFieldName)
                .SetContent(imageData);

            signerProperties.SetSignatureAppearance(appearance);
            _logger.LogInformation("Applied signature image appearance.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load signature image. Signing without visual graphic.");
        }
    }
}
