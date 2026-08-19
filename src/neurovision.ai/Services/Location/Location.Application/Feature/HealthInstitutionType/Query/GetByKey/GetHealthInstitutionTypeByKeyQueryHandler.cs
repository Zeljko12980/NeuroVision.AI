namespace LocationService.Application.Feature.HealthInstitutionType.Query.GetByKey;

public sealed class GetHealthInstitutionTypeByKeyQueryHandler
    : IQueryHandler<GetHealthInstitutionTypeByKeyQuery, Result<HealthInstitutionTypeResponse>>
{
    private readonly ILocationReadStore<HealthInstitutionTypeResponse> reads;

    public GetHealthInstitutionTypeByKeyQueryHandler(ILocationReadStore<HealthInstitutionTypeResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<HealthInstitutionTypeResponse>> Handle(
        GetHealthInstitutionTypeByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.Code }, cancellationToken);

        if (item is null)
        {
            return Result<HealthInstitutionTypeResponse>.Fail(
                "HealthInstitutionType not found.",
                HttpStatusCode.NotFound);
        }

        return Result<HealthInstitutionTypeResponse>.Ok(item);
    }
}
