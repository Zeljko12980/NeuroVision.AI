namespace MailService.Application.Common.Interfaces;

public interface IDocumentGenerator
{
    Task<Result<byte[]>> GenerateAsync(
        string templateCode,
        IReadOnlyDictionary<string, string> placeholders,
        CancellationToken cancellationToken = default);
}
