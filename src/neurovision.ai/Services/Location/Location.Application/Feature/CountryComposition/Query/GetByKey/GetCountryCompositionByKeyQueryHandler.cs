namespace LocationService.Application.Feature.CountryComposition.Query.GetByKey;

public sealed class GetCountryCompositionByKeyQueryHandler
    : IQueryHandler<GetCountryCompositionByKeyQuery, Result<CountryCompositionResponse>>
{
    private readonly ILocationReadStore<CountryCompositionResponse> reads;

    public GetCountryCompositionByKeyQueryHandler(ILocationReadStore<CountryCompositionResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<CountryCompositionResponse>> Handle(
        GetCountryCompositionByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.UnionCountryCode, query.MemberCountryCode, query.SequenceNumber }, cancellationToken);

        if (item is null)
        {
            return Result<CountryCompositionResponse>.Fail(
                "CountryComposition not found.",
                HttpStatusCode.NotFound);
        }

        return Result<CountryCompositionResponse>.Ok(item);
    }
}
