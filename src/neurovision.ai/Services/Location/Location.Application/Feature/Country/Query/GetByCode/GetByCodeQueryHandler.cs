namespace LocationService.Application.Feature.Country.Query.GetByCode;

public sealed class GetByCodeQueryHandler
    : IQueryHandler<GetByCodeQuery, Result<CountryResponse>>
{
    private readonly ILocationReadStore<CountryResponse> reads;

    public GetByCodeQueryHandler(ILocationReadStore<CountryResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<CountryResponse>> Handle(
        GetByCodeQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.Code }, cancellationToken);

        if (item is null)
        {
            return Result<CountryResponse>.Fail(
                "Country not found.",
                HttpStatusCode.NotFound);
        }

        return Result<CountryResponse>.Ok(item);
    }
}
