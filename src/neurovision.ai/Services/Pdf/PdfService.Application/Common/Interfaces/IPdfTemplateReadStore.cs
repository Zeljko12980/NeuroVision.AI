namespace PdfService.Application.Common.Interfaces;

public interface IPdfTemplateReadStore
{
    Task<Guid?> GetIdByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<PdfTemplate?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task LoadFieldsAsync(PdfTemplate template, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<PdfTemplate> Items, long TotalCount)> GetPagedAsync(
        string? code,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<PdfTemplate> Items, long TotalCount)> GetActiveAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
}
