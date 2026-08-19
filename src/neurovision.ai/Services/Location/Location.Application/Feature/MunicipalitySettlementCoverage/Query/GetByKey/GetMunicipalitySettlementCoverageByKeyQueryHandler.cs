namespace LocationService.Application.Feature.MunicipalitySettlementCoverage.Query.GetByKey;

public sealed class GetMunicipalitySettlementCoverageByKeyQueryHandler
    : IQueryHandler<GetMunicipalitySettlementCoverageByKeyQuery, Result<MunicipalitySettlementCoverageResponse>>
{
    private readonly ILocationReadStore<MunicipalitySettlementCoverageResponse> reads;

    public GetMunicipalitySettlementCoverageByKeyQueryHandler(ILocationReadStore<MunicipalitySettlementCoverageResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<MunicipalitySettlementCoverageResponse>> Handle(
        GetMunicipalitySettlementCoverageByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.CountryCode, query.MunicipalityCode, query.SettlementCode }, cancellationToken);

        if (item is null)
        {
            return Result<MunicipalitySettlementCoverageResponse>.Fail(
                "MunicipalitySettlementCoverage not found.",
                HttpStatusCode.NotFound);
        }

        return Result<MunicipalitySettlementCoverageResponse>.Ok(item);
    }
}
