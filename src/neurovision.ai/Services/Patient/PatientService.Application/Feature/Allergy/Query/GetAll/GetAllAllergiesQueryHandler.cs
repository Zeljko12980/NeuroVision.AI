namespace PatientService.Application.Feature.Allergy.Query.GetAll;

public sealed class GetAllAllergiesQueryHandler
    : IQueryHandler<GetAllAllergiesQuery, Result<PaginatedResult<AllergyResponse>>>
{
    private readonly IPatientReadStore<AllergyResponse> reads;
    private readonly ILogger<GetAllAllergiesQueryHandler> logger;

    public GetAllAllergiesQueryHandler(
        IPatientReadStore<AllergyResponse> reads,
        ILogger<GetAllAllergiesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<AllergyResponse>>> Handle(
        GetAllAllergiesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get allergies started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get allergies succeeded. Count={Count}", total);

        return Result<PaginatedResult<AllergyResponse>>.Ok(
            new PaginatedResult<AllergyResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
