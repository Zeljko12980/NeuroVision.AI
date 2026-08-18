namespace PdfService.Application.Common.Interfaces;

public interface ICertificateStorage
{
    Task<string> SaveAsync(
        byte[] content,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<string> SaveSignatureImageAsync(
        byte[] content,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<byte[]> ReadAsync(
        string relativePath,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string relativePath,
        CancellationToken cancellationToken = default);

    Task<byte[]?> TryReadSignatureImageAsync(
        string fileName,
        CancellationToken cancellationToken = default);
}
