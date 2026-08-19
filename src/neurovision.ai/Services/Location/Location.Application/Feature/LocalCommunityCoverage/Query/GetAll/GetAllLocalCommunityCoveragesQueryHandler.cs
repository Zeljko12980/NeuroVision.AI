namespace LocationService.Application.Feature.LocalCommunityCoverage.Query.GetAll;

public sealed class GetAllLocalCommunityCoveragesQueryHandler
    : IQueryHandler<GetAllLocalCommunityCoveragesQuery, Result<PaginatedResult<LocalCommunityCoverageResponse>>>
{
    private readonly ILocationReadStore<LocalCommunityCoverageResponse> reads;

    public GetAllLocalCommunityCoveragesQueryHandler(ILocationReadStore<LocalCommunityCoverageResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<LocalCommunityCoverageResponse>>> Handle(
        GetAllLocalCommunityCoveragesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(cancellationToken: cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<LocalCommunityCoverageResponse>>.Ok(
            new PaginatedResult<LocalCommunityCoverageResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
