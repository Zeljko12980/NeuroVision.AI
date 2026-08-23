namespace PatientService.Application.Feature.Patient.Query.GetAll;

public sealed class GetAllPatientsQueryHandler
    : IQueryHandler<GetAllPatientsQuery, Result<PaginatedResult<PatientResponse>>>
{
    private readonly IPatientReadStore<PatientResponse> reads;
    private readonly ILogger<GetAllPatientsQueryHandler> logger;

    public GetAllPatientsQueryHandler(
        IPatientReadStore<PatientResponse> reads,
        ILogger<GetAllPatientsQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<PatientResponse>>> Handle(
        GetAllPatientsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get patients started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get patients succeeded. Count={Count}", total);

        return Result<PaginatedResult<PatientResponse>>.Ok(
            new PaginatedResult<PatientResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
