namespace PdfService.Application.Common.Responses
{
    public sealed class GeneratePdfResponse
    {
        public required byte[] PdfBytes { get; init; }

        public bool IsSigned { get; init; }

        public Guid? CertificateId { get; init; }

        public DateTimeOffset GeneratedAt { get; init; } = DateTimeOffset.UtcNow;

        public string? SignatureReason { get; init; }

        public string? SignatureLocation { get; init; }
    }
}
