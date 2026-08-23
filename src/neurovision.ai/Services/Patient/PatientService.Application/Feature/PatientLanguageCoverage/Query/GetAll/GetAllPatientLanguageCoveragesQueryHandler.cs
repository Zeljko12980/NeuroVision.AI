namespace PatientService.Application.Feature.PatientLanguageCoverage.Query.GetAll;

public sealed class GetAllPatientLanguageCoveragesQueryHandler
    : IQueryHandler<GetAllPatientLanguageCoveragesQuery, Result<PaginatedResult<PatientLanguageCoverageResponse>>>
{
    private readonly IPatientReadStore<PatientLanguageCoverageResponse> reads;
    private readonly ILogger<GetAllPatientLanguageCoveragesQueryHandler> logger;

    public GetAllPatientLanguageCoveragesQueryHandler(
        IPatientReadStore<PatientLanguageCoverageResponse> reads,
        ILogger<GetAllPatientLanguageCoveragesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<PatientLanguageCoverageResponse>>> Handle(
        GetAllPatientLanguageCoveragesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get patient language coverages started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get patient language coverages succeeded. Count={Count}", total);

        return Result<PaginatedResult<PatientLanguageCoverageResponse>>.Ok(
            new PaginatedResult<PatientLanguageCoverageResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
