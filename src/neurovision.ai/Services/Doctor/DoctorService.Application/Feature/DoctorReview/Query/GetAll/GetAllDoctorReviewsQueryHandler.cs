namespace DoctorService.Application.Feature.DoctorReview.Query.GetAll;

public sealed class GetAllDoctorReviewsQueryHandler
    : IQueryHandler<GetAllDoctorReviewsQuery, Result<PaginatedResult<DoctorReviewResponse>>>
{
    private readonly IDoctorReadStore<DoctorReviewResponse> reads;
    private readonly ILogger<GetAllDoctorReviewsQueryHandler> logger;

    public GetAllDoctorReviewsQueryHandler(
        IDoctorReadStore<DoctorReviewResponse> reads,
        ILogger<GetAllDoctorReviewsQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<DoctorReviewResponse>>> Handle(
        GetAllDoctorReviewsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get doctor reviews started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get doctor reviews succeeded. Count={Count}", total);

        return Result<PaginatedResult<DoctorReviewResponse>>.Ok(
            new PaginatedResult<DoctorReviewResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
