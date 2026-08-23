namespace DoctorService.Application.Feature.DoctorStatus.Query.GetAll;

public sealed class GetAllDoctorStatusesQueryHandler
    : IQueryHandler<GetAllDoctorStatusesQuery, Result<PaginatedResult<DoctorStatusResponse>>>
{
    private readonly IDoctorReadStore<DoctorStatusResponse> reads;
    private readonly ILogger<GetAllDoctorStatusesQueryHandler> logger;

    public GetAllDoctorStatusesQueryHandler(
        IDoctorReadStore<DoctorStatusResponse> reads,
        ILogger<GetAllDoctorStatusesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<DoctorStatusResponse>>> Handle(
        GetAllDoctorStatusesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get doctor statuses started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get doctor statuses succeeded. Count={Count}", total);

        return Result<PaginatedResult<DoctorStatusResponse>>.Ok(
            new PaginatedResult<DoctorStatusResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
