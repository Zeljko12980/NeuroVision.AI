namespace DoctorService.Application.Feature.DoctorLanguageCoverage.Query.GetAll;

public sealed class GetAllDoctorLanguageCoveragesQueryHandler
    : IQueryHandler<GetAllDoctorLanguageCoveragesQuery, Result<PaginatedResult<DoctorLanguageCoverageResponse>>>
{
    private readonly IDoctorReadStore<DoctorLanguageCoverageResponse> reads;
    private readonly ILogger<GetAllDoctorLanguageCoveragesQueryHandler> logger;

    public GetAllDoctorLanguageCoveragesQueryHandler(
        IDoctorReadStore<DoctorLanguageCoverageResponse> reads,
        ILogger<GetAllDoctorLanguageCoveragesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<DoctorLanguageCoverageResponse>>> Handle(
        GetAllDoctorLanguageCoveragesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get doctor language coverages started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get doctor language coverages succeeded. Count={Count}", total);

        return Result<PaginatedResult<DoctorLanguageCoverageResponse>>.Ok(
            new PaginatedResult<DoctorLanguageCoverageResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
