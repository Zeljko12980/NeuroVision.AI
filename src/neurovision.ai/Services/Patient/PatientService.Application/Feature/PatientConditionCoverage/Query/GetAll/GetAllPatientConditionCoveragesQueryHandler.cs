namespace PatientService.Application.Feature.PatientConditionCoverage.Query.GetAll;

public sealed class GetAllPatientConditionCoveragesQueryHandler
    : IQueryHandler<GetAllPatientConditionCoveragesQuery, Result<PaginatedResult<PatientConditionCoverageResponse>>>
{
    private readonly IPatientReadStore<PatientConditionCoverageResponse> reads;
    private readonly ILogger<GetAllPatientConditionCoveragesQueryHandler> logger;

    public GetAllPatientConditionCoveragesQueryHandler(
        IPatientReadStore<PatientConditionCoverageResponse> reads,
        ILogger<GetAllPatientConditionCoveragesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<PatientConditionCoverageResponse>>> Handle(
        GetAllPatientConditionCoveragesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get patient condition coverages started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get patient condition coverages succeeded. Count={Count}", total);

        return Result<PaginatedResult<PatientConditionCoverageResponse>>.Ok(
            new PaginatedResult<PatientConditionCoverageResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
