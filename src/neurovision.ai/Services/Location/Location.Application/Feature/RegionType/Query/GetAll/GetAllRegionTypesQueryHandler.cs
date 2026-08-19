namespace LocationService.Application.Feature.RegionType.Query.GetAll;

public sealed class GetAllRegionTypesQueryHandler
    : IQueryHandler<GetAllRegionTypesQuery, Result<PaginatedResult<RegionTypeResponse>>>
{
    private readonly ILocationReadStore<RegionTypeResponse> reads;

    public GetAllRegionTypesQueryHandler(ILocationReadStore<RegionTypeResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<RegionTypeResponse>>> Handle(
        GetAllRegionTypesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<RegionTypeResponse>>.Ok(
            new PaginatedResult<RegionTypeResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
