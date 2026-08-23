namespace PatientService.Application.Feature.PatientAllergyCoverage.Query.GetAll;

public sealed class GetAllPatientAllergyCoveragesQueryHandler
    : IQueryHandler<GetAllPatientAllergyCoveragesQuery, Result<PaginatedResult<PatientAllergyCoverageResponse>>>
{
    private readonly IPatientReadStore<PatientAllergyCoverageResponse> reads;
    private readonly ILogger<GetAllPatientAllergyCoveragesQueryHandler> logger;

    public GetAllPatientAllergyCoveragesQueryHandler(
        IPatientReadStore<PatientAllergyCoverageResponse> reads,
        ILogger<GetAllPatientAllergyCoveragesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<PatientAllergyCoverageResponse>>> Handle(
        GetAllPatientAllergyCoveragesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get patient allergy coverages started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get patient allergy coverages succeeded. Count={Count}", total);

        return Result<PaginatedResult<PatientAllergyCoverageResponse>>.Ok(
            new PaginatedResult<PatientAllergyCoverageResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
