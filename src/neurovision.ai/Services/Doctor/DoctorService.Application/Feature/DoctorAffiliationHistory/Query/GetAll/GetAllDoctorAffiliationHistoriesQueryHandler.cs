namespace DoctorService.Application.Feature.DoctorAffiliationHistory.Query.GetAll;

public sealed class GetAllDoctorAffiliationHistoriesQueryHandler
    : IQueryHandler<GetAllDoctorAffiliationHistoriesQuery, Result<PaginatedResult<DoctorAffiliationHistoryResponse>>>
{
    private readonly IDoctorReadStore<DoctorAffiliationHistoryResponse> reads;
    private readonly ILogger<GetAllDoctorAffiliationHistoriesQueryHandler> logger;

    public GetAllDoctorAffiliationHistoriesQueryHandler(
        IDoctorReadStore<DoctorAffiliationHistoryResponse> reads,
        ILogger<GetAllDoctorAffiliationHistoriesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<DoctorAffiliationHistoryResponse>>> Handle(
        GetAllDoctorAffiliationHistoriesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get doctor affiliation histories started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get doctor affiliation histories succeeded. Count={Count}", total);

        return Result<PaginatedResult<DoctorAffiliationHistoryResponse>>.Ok(
            new PaginatedResult<DoctorAffiliationHistoryResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
