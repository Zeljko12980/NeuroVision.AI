namespace DoctorService.Application.Feature.Specialization.Query.GetAll;

public sealed class GetAllSpecializationsQueryHandler
    : IQueryHandler<GetAllSpecializationsQuery, Result<PaginatedResult<SpecializationResponse>>>
{
    private readonly IDoctorReadStore<SpecializationResponse> reads;
    private readonly ILogger<GetAllSpecializationsQueryHandler> logger;

    public GetAllSpecializationsQueryHandler(
        IDoctorReadStore<SpecializationResponse> reads,
        ILogger<GetAllSpecializationsQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<SpecializationResponse>>> Handle(
        GetAllSpecializationsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get specializations started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get specializations succeeded. Count={Count}", total);

        return Result<PaginatedResult<SpecializationResponse>>.Ok(
            new PaginatedResult<SpecializationResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
