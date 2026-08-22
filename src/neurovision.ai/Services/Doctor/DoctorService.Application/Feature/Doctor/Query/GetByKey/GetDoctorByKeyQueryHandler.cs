namespace DoctorService.Application.Feature.Doctor.Query.GetByKey;

public sealed class GetDoctorByKeyQueryHandler
    : IQueryHandler<GetDoctorByKeyQuery, Result<DoctorResponse>>
{
    private readonly IDoctorReadStore<DoctorResponse> reads;
    private readonly ILogger<GetDoctorByKeyQueryHandler> logger;

    public GetDoctorByKeyQueryHandler(
        IDoctorReadStore<DoctorResponse> reads,
        ILogger<GetDoctorByKeyQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<DoctorResponse>> Handle(
        GetDoctorByKeyQuery query,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Get doctor started. DoctorId={DoctorId}", query.Id);

        var item = await reads.GetByKeyAsync(new { query.Id }, cancellationToken);

        if (item is null)
        {
            logger.LogWarning("Get doctor failed. Doctor not found. DoctorId={DoctorId}", query.Id);
            return Result<DoctorResponse>.Fail(
                "Doctor not found.",
                HttpStatusCode.NotFound);
        }

        logger.LogInformation(
            "Get doctor succeeded. DoctorId={DoctorId}, Email={Email}",
            item.Id,
            item.Email);

        return Result<DoctorResponse>.Ok(item);
    }
}
