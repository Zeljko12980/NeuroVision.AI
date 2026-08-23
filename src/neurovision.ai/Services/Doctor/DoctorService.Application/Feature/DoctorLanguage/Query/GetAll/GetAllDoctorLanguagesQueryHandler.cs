namespace DoctorService.Application.Feature.DoctorLanguage.Query.GetAll;

public sealed class GetAllDoctorLanguagesQueryHandler
    : IQueryHandler<GetAllDoctorLanguagesQuery, Result<PaginatedResult<LanguageResponse>>>
{
    private readonly IDoctorReadStore<LanguageResponse> reads;
    private readonly ILogger<GetAllDoctorLanguagesQueryHandler> logger;

    public GetAllDoctorLanguagesQueryHandler(
        IDoctorReadStore<LanguageResponse> reads,
        ILogger<GetAllDoctorLanguagesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<LanguageResponse>>> Handle(
        GetAllDoctorLanguagesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get doctor languages started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get doctor languages succeeded. Count={Count}", total);

        return Result<PaginatedResult<LanguageResponse>>.Ok(
            new PaginatedResult<LanguageResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
