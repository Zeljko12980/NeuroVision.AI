namespace PatientService.Application.Feature.Gender.Query.GetAll;

public sealed class GetAllGendersQueryHandler
    : IQueryHandler<GetAllGendersQuery, Result<PaginatedResult<GenderResponse>>>
{
    private readonly IPatientReadStore<GenderResponse> reads;
    private readonly ILogger<GetAllGendersQueryHandler> logger;

    public GetAllGendersQueryHandler(
        IPatientReadStore<GenderResponse> reads,
        ILogger<GetAllGendersQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<GenderResponse>>> Handle(
        GetAllGendersQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get genders started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get genders succeeded. Count={Count}", total);

        return Result<PaginatedResult<GenderResponse>>.Ok(
            new PaginatedResult<GenderResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
