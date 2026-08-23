namespace PatientService.Application.Feature.Condition.Query.GetAll;

public sealed class GetAllConditionsQueryHandler
    : IQueryHandler<GetAllConditionsQuery, Result<PaginatedResult<ConditionResponse>>>
{
    private readonly IPatientReadStore<ConditionResponse> reads;
    private readonly ILogger<GetAllConditionsQueryHandler> logger;

    public GetAllConditionsQueryHandler(
        IPatientReadStore<ConditionResponse> reads,
        ILogger<GetAllConditionsQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<ConditionResponse>>> Handle(
        GetAllConditionsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get conditions started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get conditions succeeded. Count={Count}", total);

        return Result<PaginatedResult<ConditionResponse>>.Ok(
            new PaginatedResult<ConditionResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
