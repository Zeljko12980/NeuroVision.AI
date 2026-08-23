namespace DoctorService.Application.Feature.DoctorDegreeCoverage.Query.GetAll;

public sealed class GetAllDoctorDegreeCoveragesQueryHandler
    : IQueryHandler<GetAllDoctorDegreeCoveragesQuery, Result<PaginatedResult<DoctorDegreeCoverageResponse>>>
{
    private readonly IDoctorReadStore<DoctorDegreeCoverageResponse> reads;
    private readonly ILogger<GetAllDoctorDegreeCoveragesQueryHandler> logger;

    public GetAllDoctorDegreeCoveragesQueryHandler(
        IDoctorReadStore<DoctorDegreeCoverageResponse> reads,
        ILogger<GetAllDoctorDegreeCoveragesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<DoctorDegreeCoverageResponse>>> Handle(
        GetAllDoctorDegreeCoveragesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get doctor degree coverages started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get doctor degree coverages succeeded. Count={Count}", total);

        return Result<PaginatedResult<DoctorDegreeCoverageResponse>>.Ok(
            new PaginatedResult<DoctorDegreeCoverageResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
