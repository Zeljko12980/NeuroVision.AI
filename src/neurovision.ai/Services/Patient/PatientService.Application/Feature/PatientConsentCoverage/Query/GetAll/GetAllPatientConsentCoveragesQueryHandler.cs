namespace PatientService.Application.Feature.PatientConsentCoverage.Query.GetAll;

public sealed class GetAllPatientConsentCoveragesQueryHandler
    : IQueryHandler<GetAllPatientConsentCoveragesQuery, Result<PaginatedResult<PatientConsentCoverageResponse>>>
{
    private readonly IPatientReadStore<PatientConsentCoverageResponse> reads;
    private readonly ILogger<GetAllPatientConsentCoveragesQueryHandler> logger;

    public GetAllPatientConsentCoveragesQueryHandler(
        IPatientReadStore<PatientConsentCoverageResponse> reads,
        ILogger<GetAllPatientConsentCoveragesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<PatientConsentCoverageResponse>>> Handle(
        GetAllPatientConsentCoveragesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get patient consent coverages started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get patient consent coverages succeeded. Count={Count}", total);

        return Result<PaginatedResult<PatientConsentCoverageResponse>>.Ok(
            new PaginatedResult<PatientConsentCoverageResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
