namespace PdfService.Application.Common.Interfaces;

public interface IPdfGenerator
{
    Task<Result<byte[]>> GenerateFromHtmlAsync(
        string html,
        CancellationToken cancellationToken = default);
}
