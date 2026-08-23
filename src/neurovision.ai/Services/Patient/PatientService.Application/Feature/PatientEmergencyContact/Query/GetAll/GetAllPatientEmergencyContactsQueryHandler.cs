namespace PatientService.Application.Feature.PatientEmergencyContact.Query.GetAll;

public sealed class GetAllPatientEmergencyContactsQueryHandler
    : IQueryHandler<GetAllPatientEmergencyContactsQuery, Result<PaginatedResult<PatientEmergencyContactResponse>>>
{
    private readonly IPatientReadStore<PatientEmergencyContactResponse> reads;
    private readonly ILogger<GetAllPatientEmergencyContactsQueryHandler> logger;

    public GetAllPatientEmergencyContactsQueryHandler(
        IPatientReadStore<PatientEmergencyContactResponse> reads,
        ILogger<GetAllPatientEmergencyContactsQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<PatientEmergencyContactResponse>>> Handle(
        GetAllPatientEmergencyContactsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get patient emergency contacts started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get patient emergency contacts succeeded. Count={Count}", total);

        return Result<PaginatedResult<PatientEmergencyContactResponse>>.Ok(
            new PaginatedResult<PatientEmergencyContactResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
