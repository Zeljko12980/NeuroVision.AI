namespace LocationService.Application.Feature.Region.Query.GetAll;

public sealed class GetAllRegionsQueryHandler
    : IQueryHandler<GetAllRegionsQuery, Result<PaginatedResult<RegionResponse>>>
{
    private readonly ILocationReadStore<RegionResponse> reads;

    public GetAllRegionsQueryHandler(ILocationReadStore<RegionResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<RegionResponse>>> Handle(
        GetAllRegionsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<RegionResponse>>.Ok(
            new PaginatedResult<RegionResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
