namespace PatientService.Application.Feature.InsurancePayer.Query.GetAll;

public sealed class GetAllInsurancePayersQueryHandler
    : IQueryHandler<GetAllInsurancePayersQuery, Result<PaginatedResult<InsurancePayerResponse>>>
{
    private readonly IPatientReadStore<InsurancePayerResponse> reads;
    private readonly ILogger<GetAllInsurancePayersQueryHandler> logger;

    public GetAllInsurancePayersQueryHandler(
        IPatientReadStore<InsurancePayerResponse> reads,
        ILogger<GetAllInsurancePayersQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<InsurancePayerResponse>>> Handle(
        GetAllInsurancePayersQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get insurance payers started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get insurance payers succeeded. Count={Count}", total);

        return Result<PaginatedResult<InsurancePayerResponse>>.Ok(
            new PaginatedResult<InsurancePayerResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
