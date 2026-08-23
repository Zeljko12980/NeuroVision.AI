namespace AppointmentService.Application.Feature.Appointment.Query.GetRange;

public sealed class GetAppointmentRangeQueryHandler
    : IQueryHandler<GetAppointmentRangeQuery, Result<IReadOnlyList<AppointmentResponse>>>
{
    private readonly IAppointmentWriteStore writes;
    private readonly ILogger<GetAppointmentRangeQueryHandler> logger;

    public GetAppointmentRangeQueryHandler(
        IAppointmentWriteStore writes,
        ILogger<GetAppointmentRangeQueryHandler> logger)
    {
        this.writes = writes;
        this.logger = logger;
    }

    public async Task<Result<IReadOnlyList<AppointmentResponse>>> Handle(
        GetAppointmentRangeQuery query,
        CancellationToken cancellationToken)
    {
        if (query.To <= query.From)
        {
            return Result<IReadOnlyList<AppointmentResponse>>.Fail(
                "Range end must be after range start.",
                HttpStatusCode.BadRequest);
        }

        var scope = AppointmentAccess.ResolveRange(query.Actor, query.PatientId, query.DoctorId);
        if (scope.IsFailure)
        {
            return Result<IReadOnlyList<AppointmentResponse>>.Fail(
                scope.Error,
                scope.StatusCode);
        }

        var items = await writes.GetRangeAsync(
            query.From,
            query.To,
            scope.Value.PatientId,
            scope.Value.DoctorId,
            cancellationToken);

        logger.LogInformation(
            "Get appointment range succeeded. From={From}, To={To}, Count={Count}",
            query.From,
            query.To,
            items.Count);

        return Result<IReadOnlyList<AppointmentResponse>>.Ok(
            items.Select(item => item.ToResponse()).ToList());
    }
}
