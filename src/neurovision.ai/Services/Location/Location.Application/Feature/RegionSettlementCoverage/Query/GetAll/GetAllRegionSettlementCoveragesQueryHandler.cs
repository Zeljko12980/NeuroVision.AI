namespace LocationService.Application.Feature.RegionSettlementCoverage.Query.GetAll;

public sealed class GetAllRegionSettlementCoveragesQueryHandler
    : IQueryHandler<GetAllRegionSettlementCoveragesQuery, Result<PaginatedResult<RegionSettlementCoverageResponse>>>
{
    private readonly ILocationReadStore<RegionSettlementCoverageResponse> reads;

    public GetAllRegionSettlementCoveragesQueryHandler(ILocationReadStore<RegionSettlementCoverageResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<RegionSettlementCoverageResponse>>> Handle(
        GetAllRegionSettlementCoveragesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(cancellationToken: cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<RegionSettlementCoverageResponse>>.Ok(
            new PaginatedResult<RegionSettlementCoverageResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
