namespace LocationService.Application.Feature.RegionSettlementCoverage.Query.GetByKey;

public sealed class GetRegionSettlementCoverageByKeyQueryHandler
    : IQueryHandler<GetRegionSettlementCoverageByKeyQuery, Result<RegionSettlementCoverageResponse>>
{
    private readonly ILocationReadStore<RegionSettlementCoverageResponse> reads;

    public GetRegionSettlementCoverageByKeyQueryHandler(ILocationReadStore<RegionSettlementCoverageResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<RegionSettlementCoverageResponse>> Handle(
        GetRegionSettlementCoverageByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.RegionTypeCode, query.RegionCode, query.CountryCode, query.SettlementCode }, cancellationToken);

        if (item is null)
        {
            return Result<RegionSettlementCoverageResponse>.Fail(
                "RegionSettlementCoverage not found.",
                HttpStatusCode.NotFound);
        }

        return Result<RegionSettlementCoverageResponse>.Ok(item);
    }
}
