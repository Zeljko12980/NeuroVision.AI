namespace PatientService.Application.Feature.BloodType.Query.GetAll;

public sealed class GetAllBloodTypesQueryHandler
    : IQueryHandler<GetAllBloodTypesQuery, Result<PaginatedResult<BloodTypeResponse>>>
{
    private readonly IPatientReadStore<BloodTypeResponse> reads;
    private readonly ILogger<GetAllBloodTypesQueryHandler> logger;

    public GetAllBloodTypesQueryHandler(
        IPatientReadStore<BloodTypeResponse> reads,
        ILogger<GetAllBloodTypesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<BloodTypeResponse>>> Handle(
        GetAllBloodTypesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get blood types started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get blood types succeeded. Count={Count}", total);

        return Result<PaginatedResult<BloodTypeResponse>>.Ok(
            new PaginatedResult<BloodTypeResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
