namespace PatientService.Application.Feature.PatientInsuranceHistory.Query.GetAll;

public sealed class GetAllPatientInsuranceHistoriesQueryHandler
    : IQueryHandler<GetAllPatientInsuranceHistoriesQuery, Result<PaginatedResult<PatientInsuranceHistoryResponse>>>
{
    private readonly IPatientReadStore<PatientInsuranceHistoryResponse> reads;
    private readonly ILogger<GetAllPatientInsuranceHistoriesQueryHandler> logger;

    public GetAllPatientInsuranceHistoriesQueryHandler(
        IPatientReadStore<PatientInsuranceHistoryResponse> reads,
        ILogger<GetAllPatientInsuranceHistoriesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<PatientInsuranceHistoryResponse>>> Handle(
        GetAllPatientInsuranceHistoriesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get patient insurance histories started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get patient insurance histories succeeded. Count={Count}", total);

        return Result<PaginatedResult<PatientInsuranceHistoryResponse>>.Ok(
            new PaginatedResult<PatientInsuranceHistoryResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
