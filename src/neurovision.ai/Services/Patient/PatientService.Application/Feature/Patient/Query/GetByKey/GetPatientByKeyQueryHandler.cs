namespace PatientService.Application.Feature.Patient.Query.GetByKey;

public sealed class GetPatientByKeyQueryHandler
    : IQueryHandler<GetPatientByKeyQuery, Result<PatientResponse>>
{
    private readonly IPatientReadStore<PatientResponse> reads;
    private readonly ILogger<GetPatientByKeyQueryHandler> logger;

    public GetPatientByKeyQueryHandler(
        IPatientReadStore<PatientResponse> reads,
        ILogger<GetPatientByKeyQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PatientResponse>> Handle(
        GetPatientByKeyQuery query,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Get patient started. PatientId={PatientId}", query.Id);

        var item = await reads.GetByKeyAsync(new { query.Id }, cancellationToken);

        if (item is null)
        {
            logger.LogWarning("Get patient failed. Patient not found. PatientId={PatientId}", query.Id);
            return Result<PatientResponse>.Fail(
                "Patient not found.",
                HttpStatusCode.NotFound);
        }

        logger.LogInformation(
            "Get patient succeeded. PatientId={PatientId}, Email={Email}",
            item.Id,
            item.Email);

        return Result<PatientResponse>.Ok(item);
    }
}
