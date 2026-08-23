namespace DoctorService.Application.Feature.DoctorSpecializationCoverage.Query.GetAll;

public sealed class GetAllDoctorSpecializationCoveragesQueryHandler
    : IQueryHandler<GetAllDoctorSpecializationCoveragesQuery, Result<PaginatedResult<DoctorSpecializationCoverageResponse>>>
{
    private readonly IDoctorReadStore<DoctorSpecializationCoverageResponse> reads;
    private readonly ILogger<GetAllDoctorSpecializationCoveragesQueryHandler> logger;

    public GetAllDoctorSpecializationCoveragesQueryHandler(
        IDoctorReadStore<DoctorSpecializationCoverageResponse> reads,
        ILogger<GetAllDoctorSpecializationCoveragesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<DoctorSpecializationCoverageResponse>>> Handle(
        GetAllDoctorSpecializationCoveragesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get doctor specialization coverages started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get doctor specialization coverages succeeded. Count={Count}", total);

        return Result<PaginatedResult<DoctorSpecializationCoverageResponse>>.Ok(
            new PaginatedResult<DoctorSpecializationCoverageResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
