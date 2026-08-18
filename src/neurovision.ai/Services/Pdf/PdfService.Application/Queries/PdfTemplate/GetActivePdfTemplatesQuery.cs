namespace PdfService.Application.Queries.Templates;

public sealed record GetActivePdfTemplatesQuery(PaginationRequest Request)
    : IQuery<Result<PaginatedResult<PdfTemplateResponse>>>;

public sealed class GetActivePdfTemplatesQueryHandler
    : IQueryHandler<GetActivePdfTemplatesQuery, Result<PaginatedResult<PdfTemplateResponse>>>
{
    private readonly IPdfTemplateReadStore _readStore;

    public GetActivePdfTemplatesQueryHandler(IPdfTemplateReadStore readStore)
    {
        _readStore = readStore;
    }

    public async Task<Result<PaginatedResult<PdfTemplateResponse>>> Handle(
        GetActivePdfTemplatesQuery query,
        CancellationToken cancellationToken)
    {
        var pageIndex = Math.Max(query.Request.PageIndex, 0);
        var (items, totalCount) = await _readStore.GetActiveAsync(
            pageIndex,
            query.Request.PageSize,
            cancellationToken);

        return Result<PaginatedResult<PdfTemplateResponse>>.Ok(
            new PaginatedResult<PdfTemplateResponse>(
                pageIndex,
                query.Request.PageSize,
                totalCount,
                items.Select(item => item.ToResponse()).ToList()));
    }
}
