namespace PdfService.Application.Queries.Templates;

public sealed record GetAllPdfTemplatesQuery(GetPdfTemplatesRequest Request)
    : IQuery<Result<PaginatedResult<PdfTemplateResponse>>>;

public sealed class GetAllPdfTemplatesQueryHandler
    : IQueryHandler<GetAllPdfTemplatesQuery, Result<PaginatedResult<PdfTemplateResponse>>>
{
    private readonly IPdfTemplateReadStore _readStore;

    public GetAllPdfTemplatesQueryHandler(IPdfTemplateReadStore readStore)
    {
        _readStore = readStore;
    }

    public async Task<Result<PaginatedResult<PdfTemplateResponse>>> Handle(
        GetAllPdfTemplatesQuery query,
        CancellationToken cancellationToken)
    {
        var pageIndex = Math.Max(query.Request.PageIndex, 0);
        var (items, totalCount) = await _readStore.GetPagedAsync(
            query.Request.Code,
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
