namespace PdfService.Application.Common.Interfaces;

public interface IPdfSigningService
{
    SignaturePosition ResolvePosition(byte[] pdfBytes, PdfTemplate template);

    Task<Result<byte[]>> SignPdfAsync(
        byte[] pdfBytes,
        Guid certificateId,
        SignaturePosition position,
        string? reason,
        string? location,
        CancellationToken cancellationToken = default);
}
