namespace PatientService.Application.Feature.PatientDoctorAssignmentHistory.Query.GetAll;

public sealed class GetAllPatientDoctorAssignmentHistoriesQueryHandler
    : IQueryHandler<GetAllPatientDoctorAssignmentHistoriesQuery, Result<PaginatedResult<PatientDoctorAssignmentHistoryResponse>>>
{
    private readonly IPatientReadStore<PatientDoctorAssignmentHistoryResponse> reads;
    private readonly ILogger<GetAllPatientDoctorAssignmentHistoriesQueryHandler> logger;

    public GetAllPatientDoctorAssignmentHistoriesQueryHandler(
        IPatientReadStore<PatientDoctorAssignmentHistoryResponse> reads,
        ILogger<GetAllPatientDoctorAssignmentHistoriesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<PatientDoctorAssignmentHistoryResponse>>> Handle(
        GetAllPatientDoctorAssignmentHistoriesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get patient doctor assignments started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get patient doctor assignments succeeded. Count={Count}", total);

        return Result<PaginatedResult<PatientDoctorAssignmentHistoryResponse>>.Ok(
            new PaginatedResult<PatientDoctorAssignmentHistoryResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
