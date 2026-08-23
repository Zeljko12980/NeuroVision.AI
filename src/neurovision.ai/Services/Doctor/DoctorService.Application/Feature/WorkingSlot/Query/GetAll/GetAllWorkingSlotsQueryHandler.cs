namespace DoctorService.Application.Feature.WorkingSlot.Query.GetAll;

public sealed class GetAllWorkingSlotsQueryHandler
    : IQueryHandler<GetAllWorkingSlotsQuery, Result<PaginatedResult<WorkingSlotResponse>>>
{
    private readonly IDoctorReadStore<WorkingSlotResponse> reads;
    private readonly ILogger<GetAllWorkingSlotsQueryHandler> logger;

    public GetAllWorkingSlotsQueryHandler(
        IDoctorReadStore<WorkingSlotResponse> reads,
        ILogger<GetAllWorkingSlotsQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<WorkingSlotResponse>>> Handle(
        GetAllWorkingSlotsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get working slots started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get working slots succeeded. Count={Count}", total);

        return Result<PaginatedResult<WorkingSlotResponse>>.Ok(
            new PaginatedResult<WorkingSlotResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
