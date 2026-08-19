namespace LocationService.Application.Feature.Municipality.Query.GetByKey;

public sealed class GetMunicipalityByKeyQueryHandler
    : IQueryHandler<GetMunicipalityByKeyQuery, Result<MunicipalityResponse>>
{
    private readonly ILocationReadStore<MunicipalityResponse> reads;

    public GetMunicipalityByKeyQueryHandler(ILocationReadStore<MunicipalityResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<MunicipalityResponse>> Handle(
        GetMunicipalityByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.CountryCode, query.Code }, cancellationToken);

        if (item is null)
        {
            return Result<MunicipalityResponse>.Fail(
                "Municipality not found.",
                HttpStatusCode.NotFound);
        }

        return Result<MunicipalityResponse>.Ok(item);
    }
}
