namespace PdfService.Application.Common.Models;

public sealed class ParsedCertificate
{
    public required string Subject { get; init; }

    public required string Issuer { get; init; }

    public required string Thumbprint { get; init; }

    public required string SerialNumber { get; init; }

    public DateTime ValidFrom { get; init; }

    public DateTime ValidTo { get; init; }
}
