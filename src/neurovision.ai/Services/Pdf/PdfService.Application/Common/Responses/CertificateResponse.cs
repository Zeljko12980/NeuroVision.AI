namespace PdfService.Application.Common.Responses;

public class CertificateResponse
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public Guid? UserId { get; init; }

    public required string Subject { get; init; }

    public required string Issuer { get; init; }

    public required string Thumbprint { get; init; }

    public required string SerialNumber { get; init; }

    public DateTime ValidFrom { get; init; }

    public DateTime ValidTo { get; init; }

    public required string FileName { get; init; }

    public required string FilePath { get; init; }

    public string? SignatureImagePath { get; init; }

    public bool HasSignatureImage => !string.IsNullOrWhiteSpace(SignatureImagePath);

    public bool IsDefault { get; init; }

    public bool IsExpired => ValidTo < DateTime.UtcNow;
}
