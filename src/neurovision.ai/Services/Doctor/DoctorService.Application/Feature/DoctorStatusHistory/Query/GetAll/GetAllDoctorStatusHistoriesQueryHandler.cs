namespace DoctorService.Application.Feature.DoctorStatusHistory.Query.GetAll;

public sealed class GetAllDoctorStatusHistoriesQueryHandler
    : IQueryHandler<GetAllDoctorStatusHistoriesQuery, Result<PaginatedResult<DoctorStatusHistoryResponse>>>
{
    private readonly IDoctorReadStore<DoctorStatusHistoryResponse> reads;
    private readonly ILogger<GetAllDoctorStatusHistoriesQueryHandler> logger;

    public GetAllDoctorStatusHistoriesQueryHandler(
        IDoctorReadStore<DoctorStatusHistoryResponse> reads,
        ILogger<GetAllDoctorStatusHistoriesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<DoctorStatusHistoryResponse>>> Handle(
        GetAllDoctorStatusHistoriesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get doctor status histories started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get doctor status histories succeeded. Count={Count}", total);

        return Result<PaginatedResult<DoctorStatusHistoryResponse>>.Ok(
            new PaginatedResult<DoctorStatusHistoryResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
