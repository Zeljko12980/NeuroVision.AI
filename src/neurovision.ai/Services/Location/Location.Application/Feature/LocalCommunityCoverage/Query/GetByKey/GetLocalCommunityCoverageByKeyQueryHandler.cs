namespace LocationService.Application.Feature.LocalCommunityCoverage.Query.GetByKey;

public sealed class GetLocalCommunityCoverageByKeyQueryHandler
    : IQueryHandler<GetLocalCommunityCoverageByKeyQuery, Result<LocalCommunityCoverageResponse>>
{
    private readonly ILocationReadStore<LocalCommunityCoverageResponse> reads;

    public GetLocalCommunityCoverageByKeyQueryHandler(ILocationReadStore<LocalCommunityCoverageResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<LocalCommunityCoverageResponse>> Handle(
        GetLocalCommunityCoverageByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.CountryCode, query.MunicipalityCode, query.LocalCommunityIdentifier, query.SettlementCode }, cancellationToken);

        if (item is null)
        {
            return Result<LocalCommunityCoverageResponse>.Fail(
                "LocalCommunityCoverage not found.",
                HttpStatusCode.NotFound);
        }

        return Result<LocalCommunityCoverageResponse>.Ok(item);
    }
}
