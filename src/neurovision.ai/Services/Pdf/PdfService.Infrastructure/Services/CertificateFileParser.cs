using BuildingBlocks.Results;
using PdfService.Application.Common.Interfaces;
using PdfService.Application.Common.Models;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace PdfService.Infrastructure.Services;

public sealed class CertificateFileParser : ICertificateFileParser
{
    public Result<ParsedCertificate> Parse(byte[] rawData, string? password)
    {
        try
        {
            using var parsed = string.IsNullOrEmpty(password)
                ? X509CertificateLoader.LoadCertificate(rawData)
                : X509CertificateLoader.LoadPkcs12(
                    rawData,
                    password,
                    X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);

            return Result<ParsedCertificate>.Ok(new ParsedCertificate
            {
                Subject = parsed.Subject,
                Issuer = parsed.Issuer,
                Thumbprint = parsed.Thumbprint,
                SerialNumber = parsed.SerialNumber,
                ValidFrom = parsed.NotBefore.ToUniversalTime(),
                ValidTo = parsed.NotAfter.ToUniversalTime()
            });
        }
        catch (CryptographicException ex) when (IsWrongPasswordError(ex))
        {
            return Result<ParsedCertificate>.Fail(
                "The certificate password is incorrect.",
                HttpStatusCode.BadRequest);
        }
        catch (CryptographicException)
        {
            return Result<ParsedCertificate>.Fail(
                "The file is not a valid certificate or is corrupted.",
                HttpStatusCode.BadRequest);
        }
    }

    private static bool IsWrongPasswordError(CryptographicException ex)
    {
        const int WrongPasswordHResultWindows = unchecked((int)0x80070056);
        const int WrongPasswordHResultOpenSsl = unchecked((int)0x8009000D);

        return ex.HResult == WrongPasswordHResultWindows
            || ex.HResult == WrongPasswordHResultOpenSsl;
    }
}
