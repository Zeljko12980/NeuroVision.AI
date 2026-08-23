namespace PatientService.Application.Feature.PatientAffiliationHistory.Query.GetAll;

public sealed class GetAllPatientAffiliationHistoriesQueryHandler
    : IQueryHandler<GetAllPatientAffiliationHistoriesQuery, Result<PaginatedResult<PatientAffiliationHistoryResponse>>>
{
    private readonly IPatientReadStore<PatientAffiliationHistoryResponse> reads;
    private readonly ILogger<GetAllPatientAffiliationHistoriesQueryHandler> logger;

    public GetAllPatientAffiliationHistoriesQueryHandler(
        IPatientReadStore<PatientAffiliationHistoryResponse> reads,
        ILogger<GetAllPatientAffiliationHistoriesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<PatientAffiliationHistoryResponse>>> Handle(
        GetAllPatientAffiliationHistoriesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get patient affiliation histories started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get patient affiliation histories succeeded. Count={Count}", total);

        return Result<PaginatedResult<PatientAffiliationHistoryResponse>>.Ok(
            new PaginatedResult<PatientAffiliationHistoryResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
