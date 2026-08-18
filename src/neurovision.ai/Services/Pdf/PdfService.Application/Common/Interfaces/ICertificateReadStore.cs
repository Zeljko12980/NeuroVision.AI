namespace PdfService.Application.Common.Interfaces;

public interface ICertificateReadStore
{
    Task<(IReadOnlyList<Certificate> Items, long TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Certificate?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Certificate?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
