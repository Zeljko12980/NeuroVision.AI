namespace DoctorService.Application.Feature.Doctor.Query.GetAll;

public sealed class GetAllDoctorsQueryHandler
    : IQueryHandler<GetAllDoctorsQuery, Result<PaginatedResult<DoctorResponse>>>
{
    private readonly IDoctorReadStore<DoctorResponse> reads;
    private readonly ILogger<GetAllDoctorsQueryHandler> logger;

    public GetAllDoctorsQueryHandler(
        IDoctorReadStore<DoctorResponse> reads,
        ILogger<GetAllDoctorsQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<DoctorResponse>>> Handle(
        GetAllDoctorsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get doctors started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get doctors succeeded. Count={Count}", total);

        return Result<PaginatedResult<DoctorResponse>>.Ok(
            new PaginatedResult<DoctorResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
