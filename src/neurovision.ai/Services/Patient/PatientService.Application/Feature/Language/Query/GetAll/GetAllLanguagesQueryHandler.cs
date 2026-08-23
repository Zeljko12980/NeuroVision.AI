namespace PatientService.Application.Feature.Language.Query.GetAll;

public sealed class GetAllLanguagesQueryHandler
    : IQueryHandler<GetAllLanguagesQuery, Result<PaginatedResult<LanguageResponse>>>
{
    private readonly IPatientReadStore<LanguageResponse> reads;
    private readonly ILogger<GetAllLanguagesQueryHandler> logger;

    public GetAllLanguagesQueryHandler(
        IPatientReadStore<LanguageResponse> reads,
        ILogger<GetAllLanguagesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<LanguageResponse>>> Handle(
        GetAllLanguagesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get languages started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get languages succeeded. Count={Count}", total);

        return Result<PaginatedResult<LanguageResponse>>.Ok(
            new PaginatedResult<LanguageResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
