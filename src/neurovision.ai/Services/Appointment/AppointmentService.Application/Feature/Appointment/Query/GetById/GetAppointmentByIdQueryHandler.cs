namespace AppointmentService.Application.Feature.Appointment.Query.GetById;

public sealed class GetAppointmentByIdQueryHandler
    : IQueryHandler<GetAppointmentByIdQuery, Result<AppointmentResponse>>
{
    private readonly IAppointmentWriteStore writes;
    private readonly ILogger<GetAppointmentByIdQueryHandler> logger;

    public GetAppointmentByIdQueryHandler(
        IAppointmentWriteStore writes,
        ILogger<GetAppointmentByIdQueryHandler> logger)
    {
        this.writes = writes;
        this.logger = logger;
    }

    public async Task<Result<AppointmentResponse>> Handle(
        GetAppointmentByIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.Id == Guid.Empty)
        {
            return Result<AppointmentResponse>.Fail(
                "Appointment id is required.",
                HttpStatusCode.BadRequest);
        }

        var appointment = await writes.FindAsync(query.Id, cancellationToken);
        if (appointment is null || !AppointmentAccess.CanAccess(query.Actor, appointment))
        {
            return Result<AppointmentResponse>.Fail(
                "Appointment was not found.",
                HttpStatusCode.NotFound);
        }

        logger.LogInformation("Get appointment succeeded. AppointmentId={AppointmentId}", appointment.Id);
        return Result<AppointmentResponse>.Ok(appointment.ToResponse());
    }
}
