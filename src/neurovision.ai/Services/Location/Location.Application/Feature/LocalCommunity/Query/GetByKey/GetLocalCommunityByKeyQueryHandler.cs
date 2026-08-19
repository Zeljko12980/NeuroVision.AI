namespace LocationService.Application.Feature.LocalCommunity.Query.GetByKey;

public sealed class GetLocalCommunityByKeyQueryHandler
    : IQueryHandler<GetLocalCommunityByKeyQuery, Result<LocalCommunityResponse>>
{
    private readonly ILocationReadStore<LocalCommunityResponse> reads;

    public GetLocalCommunityByKeyQueryHandler(ILocationReadStore<LocalCommunityResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<LocalCommunityResponse>> Handle(
        GetLocalCommunityByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.CountryCode, query.MunicipalityCode, query.Identifier }, cancellationToken);

        if (item is null)
        {
            return Result<LocalCommunityResponse>.Fail(
                "LocalCommunity not found.",
                HttpStatusCode.NotFound);
        }

        return Result<LocalCommunityResponse>.Ok(item);
    }
}
