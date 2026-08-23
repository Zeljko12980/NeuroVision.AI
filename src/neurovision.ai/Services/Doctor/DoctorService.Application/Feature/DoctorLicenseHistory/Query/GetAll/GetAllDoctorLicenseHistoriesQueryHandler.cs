namespace DoctorService.Application.Feature.DoctorLicenseHistory.Query.GetAll;

public sealed class GetAllDoctorLicenseHistoriesQueryHandler
    : IQueryHandler<GetAllDoctorLicenseHistoriesQuery, Result<PaginatedResult<DoctorLicenseHistoryResponse>>>
{
    private readonly IDoctorReadStore<DoctorLicenseHistoryResponse> reads;
    private readonly ILogger<GetAllDoctorLicenseHistoriesQueryHandler> logger;

    public GetAllDoctorLicenseHistoriesQueryHandler(
        IDoctorReadStore<DoctorLicenseHistoryResponse> reads,
        ILogger<GetAllDoctorLicenseHistoriesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<DoctorLicenseHistoryResponse>>> Handle(
        GetAllDoctorLicenseHistoriesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get doctor license histories started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get doctor license histories succeeded. Count={Count}", total);

        return Result<PaginatedResult<DoctorLicenseHistoryResponse>>.Ok(
            new PaginatedResult<DoctorLicenseHistoryResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
