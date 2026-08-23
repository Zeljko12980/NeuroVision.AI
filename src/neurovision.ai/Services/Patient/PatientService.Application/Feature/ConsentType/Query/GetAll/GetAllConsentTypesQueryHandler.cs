namespace PatientService.Application.Feature.ConsentType.Query.GetAll;

public sealed class GetAllConsentTypesQueryHandler
    : IQueryHandler<GetAllConsentTypesQuery, Result<PaginatedResult<ConsentTypeResponse>>>
{
    private readonly IPatientReadStore<ConsentTypeResponse> reads;
    private readonly ILogger<GetAllConsentTypesQueryHandler> logger;

    public GetAllConsentTypesQueryHandler(
        IPatientReadStore<ConsentTypeResponse> reads,
        ILogger<GetAllConsentTypesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<ConsentTypeResponse>>> Handle(
        GetAllConsentTypesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get consent types started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get consent types succeeded. Count={Count}", total);

        return Result<PaginatedResult<ConsentTypeResponse>>.Ok(
            new PaginatedResult<ConsentTypeResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
