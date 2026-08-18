namespace PdfService.Application.Common.Interfaces;

public interface ICertificateReadStore
{
    Task<(IReadOnlyList<Certificate> Items, long TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
}
