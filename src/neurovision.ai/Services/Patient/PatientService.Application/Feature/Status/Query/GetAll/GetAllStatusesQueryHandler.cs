namespace PatientService.Application.Feature.Status.Query.GetAll;

public sealed class GetAllStatusesQueryHandler
    : IQueryHandler<GetAllStatusesQuery, Result<PaginatedResult<PatientStatusResponse>>>
{
    private readonly IPatientReadStore<PatientStatusResponse> reads;
    private readonly ILogger<GetAllStatusesQueryHandler> logger;

    public GetAllStatusesQueryHandler(
        IPatientReadStore<PatientStatusResponse> reads,
        ILogger<GetAllStatusesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<PatientStatusResponse>>> Handle(
        GetAllStatusesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get patient statuses started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get patient statuses succeeded. Count={Count}", total);

        return Result<PaginatedResult<PatientStatusResponse>>.Ok(
            new PaginatedResult<PatientStatusResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
