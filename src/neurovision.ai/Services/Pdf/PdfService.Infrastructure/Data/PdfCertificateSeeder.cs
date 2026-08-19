using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PdfService.Application.Common.Interfaces;
using PdfService.Domain.Entities;

namespace PdfService.Infrastructure.Data;

public static class PdfCertificateSeeder
{
    private const int MinimumSignatureImageBytes = 500;

    public static async Task SeedAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PdfDbContext>();
        var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var passwordProtector = scope.ServiceProvider.GetRequiredService<ICertificatePasswordProtector>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<PdfDbContext>>();

        EnsureSignatureImage(environment, logger);

        var existing = await db.Certificates
            .FirstOrDefaultAsync(x => x.Id == PdfSeedConstants.DefaultDoctorCertificateId, cancellationToken);

        if (existing is not null)
        {
            await EnsureCertificateFileAvailableAsync(
                existing,
                environment,
                passwordProtector,
                db,
                logger,
                cancellationToken);
            return;
        }

        await SeedDefaultCertificateAsync(
            db,
            environment,
            passwordProtector,
            logger,
            cancellationToken);
    }

    private static async Task SeedDefaultCertificateAsync(
        PdfDbContext db,
        IWebHostEnvironment environment,
        ICertificatePasswordProtector passwordProtector,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var webRoot = ResolveWebRoot(environment);
        var certificatesDir = Path.Combine(webRoot, "certificates");
        Directory.CreateDirectory(certificatesDir);

        var absoluteCertPath = Path.Combine(
            certificatesDir,
            PdfSeedConstants.DefaultCertificateFileName);

        if (!File.Exists(absoluteCertPath))
        {
            GenerateDevCertificateFile(absoluteCertPath, PdfSeedConstants.DefaultCertificatePassword);
            logger.LogInformation(
                "Generated development signing certificate at {Path}",
                absoluteCertPath);
        }

        var pfxBytes = await File.ReadAllBytesAsync(absoluteCertPath, cancellationToken);
        using var loaded = X509CertificateLoader.LoadPkcs12(
            pfxBytes,
            PdfSeedConstants.DefaultCertificatePassword,
            X509KeyStorageFlags.Exportable);

        var relativePath =
            $"certificates/{PdfSeedConstants.DefaultCertificateFileName}";

        var certificate = Certificate.Create(
            PdfSeedConstants.DefaultCertificateName,
            loaded.Subject,
            loaded.Issuer,
            loaded.Thumbprint ?? string.Empty,
            loaded.SerialNumber ?? string.Empty,
            loaded.NotBefore.ToUniversalTime(),
            loaded.NotAfter.ToUniversalTime(),
            PdfSeedConstants.DefaultCertificateFileName,
            relativePath,
            passwordProtector.Protect(PdfSeedConstants.DefaultCertificatePassword),
            isDefault: true,
            id: PdfSeedConstants.DefaultDoctorCertificateId);

        await db.Certificates.AddAsync(certificate, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seeded default doctor signing certificate (Id={CertificateId}, Thumbprint={Thumbprint}).",
            certificate.Id,
            certificate.Thumbprint);
    }

    private static async Task EnsureCertificateFileAvailableAsync(
        Certificate existing,
        IWebHostEnvironment environment,
        ICertificatePasswordProtector passwordProtector,
        PdfDbContext db,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var webRoot = ResolveWebRoot(environment);
        var certificatesDir = Path.Combine(webRoot, "certificates");
        Directory.CreateDirectory(certificatesDir);

        var absoluteCertPath = Path.Combine(certificatesDir, PdfSeedConstants.DefaultCertificateFileName);
        var needsRegeneration = !File.Exists(absoluteCertPath);

        if (!needsRegeneration)
        {
            needsRegeneration = !CanUnlockCertificate(absoluteCertPath, existing, passwordProtector, logger);
        }

        if (needsRegeneration)
        {
            GenerateDevCertificateFile(absoluteCertPath, PdfSeedConstants.DefaultCertificatePassword);

            var pfxBytes = await File.ReadAllBytesAsync(absoluteCertPath, cancellationToken);
            using var loaded = X509CertificateLoader.LoadPkcs12(
                pfxBytes,
                PdfSeedConstants.DefaultCertificatePassword,
                X509KeyStorageFlags.Exportable);

            existing.UpdateMetadata(
                loaded.Subject,
                loaded.Issuer,
                loaded.Thumbprint ?? string.Empty,
                loaded.SerialNumber ?? string.Empty,
                loaded.NotBefore.ToUniversalTime(),
                loaded.NotAfter.ToUniversalTime());

            existing.UpdateFilePath($"certificates/{PdfSeedConstants.DefaultCertificateFileName}");
            existing.UpdateProtectedPassword(
                passwordProtector.Protect(PdfSeedConstants.DefaultCertificatePassword));

            await db.SaveChangesAsync(cancellationToken);

            logger.LogWarning(
                "Regenerated and re-synced development signing certificate (Id={CertificateId}).",
                existing.Id);
        }
        else
        {
            logger.LogInformation(
                "Development signing certificate already present and unlockable (Id={CertificateId}).",
                existing.Id);
        }
    }

    private static bool CanUnlockCertificate(
        string absoluteCertPath,
        Certificate existing,
        ICertificatePasswordProtector passwordProtector,
        ILogger logger)
    {
        try
        {
            var password = passwordProtector.Unprotect(existing.ProtectedPassword);
            var pfxBytes = File.ReadAllBytes(absoluteCertPath);
            using var cert = X509CertificateLoader.LoadPkcs12(
                pfxBytes,
                password,
                X509KeyStorageFlags.Exportable);

            return cert.HasPrivateKey;
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Stored certificate password does not unlock PFX at {Path}. Will regenerate.",
                absoluteCertPath);
            return false;
        }
    }

    private static void EnsureSignatureImage(IWebHostEnvironment environment, ILogger logger)
    {
        var webRoot = ResolveWebRoot(environment);
        var signaturesDir = Path.Combine(webRoot, "signatures");
        Directory.CreateDirectory(signaturesDir);

        var signaturePath = Path.Combine(signaturesDir, PdfSeedConstants.SignatureFileName);
        var bundled = Path.Combine(
            environment.ContentRootPath,
            "wwwroot",
            "signatures",
            PdfSeedConstants.SignatureFileName);

        if (File.Exists(bundled))
        {
            var bundledInfo = new FileInfo(bundled);
            var shouldCopy = !File.Exists(signaturePath)
                || new FileInfo(signaturePath).Length < MinimumSignatureImageBytes
                || bundledInfo.Length > new FileInfo(signaturePath).Length;

            if (shouldCopy)
            {
                File.Copy(bundled, signaturePath, overwrite: true);
                logger.LogInformation("Ensured signature image at {Path} from bundled asset.", signaturePath);
                return;
            }
        }

        if (File.Exists(signaturePath) && new FileInfo(signaturePath).Length >= MinimumSignatureImageBytes)
            return;

        WriteFallbackSignaturePng(signaturePath);
        logger.LogInformation("Created fallback signature image at {Path}", signaturePath);
    }

    private static void GenerateDevCertificateFile(string absolutePath, string password)
    {
        using var rsa = RSA.Create(2048);
        var subject = new X500DistinguishedName(
            "CN=NeuroVision Doctor Dev, O=NeuroVision.AI, C=RS");
        var request = new CertificateRequest(
            subject,
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, critical: true));

        var notBefore = DateTimeOffset.UtcNow.AddDays(-1);
        var notAfter = DateTimeOffset.UtcNow.AddYears(5);
        using var certificate = request.CreateSelfSigned(notBefore, notAfter);
        var pfxBytes = certificate.Export(X509ContentType.Pfx, password);
        File.WriteAllBytes(absolutePath, pfxBytes);
    }

    private static void WriteFallbackSignaturePng(string signaturePath) =>
        File.WriteAllBytes(signaturePath, MinimalSignaturePng);

    private static string ResolveWebRoot(IWebHostEnvironment environment) =>
        string.IsNullOrWhiteSpace(environment.WebRootPath)
            ? Path.Combine(environment.ContentRootPath, "wwwroot")
            : environment.WebRootPath;

    // 1x1 blue pixel PNG — last-resort placeholder
    private static readonly byte[] MinimalSignaturePng =
    [
        0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
        0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
        0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4, 0x89, 0x00, 0x00, 0x00,
        0x0A, 0x49, 0x44, 0x41, 0x54, 0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00,
        0x05, 0x00, 0x01, 0x0D, 0x0A, 0x2D, 0xB4, 0x00, 0x00, 0x00, 0x00, 0x49,
        0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82,
    ];
}
