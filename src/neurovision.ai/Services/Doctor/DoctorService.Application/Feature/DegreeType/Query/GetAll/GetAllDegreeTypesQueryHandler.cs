namespace DoctorService.Application.Feature.DegreeType.Query.GetAll;

public sealed class GetAllDegreeTypesQueryHandler
    : IQueryHandler<GetAllDegreeTypesQuery, Result<PaginatedResult<DegreeTypeResponse>>>
{
    private readonly IDoctorReadStore<DegreeTypeResponse> reads;
    private readonly ILogger<GetAllDegreeTypesQueryHandler> logger;

    public GetAllDegreeTypesQueryHandler(
        IDoctorReadStore<DegreeTypeResponse> reads,
        ILogger<GetAllDegreeTypesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<DegreeTypeResponse>>> Handle(
        GetAllDegreeTypesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get degree types started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get degree types succeeded. Count={Count}", total);

        return Result<PaginatedResult<DegreeTypeResponse>>.Ok(
            new PaginatedResult<DegreeTypeResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
