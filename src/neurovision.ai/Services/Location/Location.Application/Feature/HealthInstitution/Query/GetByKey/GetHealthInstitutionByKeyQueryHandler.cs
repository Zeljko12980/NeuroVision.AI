namespace LocationService.Application.Feature.HealthInstitution.Query.GetByKey;

public sealed class GetHealthInstitutionByKeyQueryHandler
    : IQueryHandler<GetHealthInstitutionByKeyQuery, Result<HealthInstitutionResponse>>
{
    private readonly ILocationReadStore<HealthInstitutionResponse> reads;

    public GetHealthInstitutionByKeyQueryHandler(ILocationReadStore<HealthInstitutionResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<HealthInstitutionResponse>> Handle(
        GetHealthInstitutionByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.Id }, cancellationToken);

        if (item is null)
        {
            return Result<HealthInstitutionResponse>.Fail(
                "HealthInstitution not found.",
                HttpStatusCode.NotFound);
        }

        return Result<HealthInstitutionResponse>.Ok(item);
    }
}
