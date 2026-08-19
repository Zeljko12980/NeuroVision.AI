namespace PdfService.Application.Queries.Certificates;

public sealed record GetAllCertificatesQuery(PaginationRequest Request)
    : IQuery<Result<PaginatedResult<CertificateResponse>>>;

public sealed class GetAllCertificatesQueryHandler
    : IQueryHandler<GetAllCertificatesQuery, Result<PaginatedResult<CertificateResponse>>>
{
    private readonly ICertificateReadStore _readStore;

    public GetAllCertificatesQueryHandler(ICertificateReadStore readStore)
    {
        _readStore = readStore;
    }

    public async Task<Result<PaginatedResult<CertificateResponse>>> Handle(
        GetAllCertificatesQuery query,
        CancellationToken cancellationToken)
    {
        var pageIndex = Math.Max(query.Request.PageIndex, 0);
        var (items, totalCount) = await _readStore.GetPagedAsync(
            pageIndex,
            query.Request.PageSize,
            cancellationToken);

        return Result<PaginatedResult<CertificateResponse>>.Ok(
            new PaginatedResult<CertificateResponse>(
                pageIndex,
                query.Request.PageSize,
                totalCount,
                items.Select(item => item.ToResponse()).ToList()));
    }
}
