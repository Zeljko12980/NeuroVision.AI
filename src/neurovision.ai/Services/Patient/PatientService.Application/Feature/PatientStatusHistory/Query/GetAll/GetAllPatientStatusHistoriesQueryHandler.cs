namespace PatientService.Application.Feature.PatientStatusHistory.Query.GetAll;

public sealed class GetAllPatientStatusHistoriesQueryHandler
    : IQueryHandler<GetAllPatientStatusHistoriesQuery, Result<PaginatedResult<PatientStatusHistoryResponse>>>
{
    private readonly IPatientReadStore<PatientStatusHistoryResponse> reads;
    private readonly ILogger<GetAllPatientStatusHistoriesQueryHandler> logger;

    public GetAllPatientStatusHistoriesQueryHandler(
        IPatientReadStore<PatientStatusHistoryResponse> reads,
        ILogger<GetAllPatientStatusHistoriesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<PatientStatusHistoryResponse>>> Handle(
        GetAllPatientStatusHistoriesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get patient status histories started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get patient status histories succeeded. Count={Count}", total);

        return Result<PaginatedResult<PatientStatusHistoryResponse>>.Ok(
            new PaginatedResult<PatientStatusHistoryResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
