namespace AppointmentService.Application.Feature.Appointment.Command.Cancel;

public sealed class CancelAppointmentCommandHandler
    : ICommandHandler<CancelAppointmentCommand, Result<AppointmentResponse>>
{
    private readonly IAppointmentWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly IPublishEndpoint publishEndpoint;
    private readonly ILogger<CancelAppointmentCommandHandler> logger;

    public CancelAppointmentCommandHandler(
        IAppointmentWriteStore writes,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint,
        ILogger<CancelAppointmentCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.publishEndpoint = publishEndpoint;
        this.logger = logger;
    }

    public async Task<Result<AppointmentResponse>> Handle(
        CancelAppointmentCommand command,
        CancellationToken cancellationToken)
    {
        var appointment = await writes.FindAsync(command.Id, cancellationToken);
        if (appointment is null)
        {
            return Result<AppointmentResponse>.Fail(
                "Appointment was not found.",
                HttpStatusCode.NotFound);
        }

        var authorized = AppointmentAccess.AuthorizeMutate(command.Actor, appointment);
        if (authorized.IsFailure)
            return Result<AppointmentResponse>.Fail(authorized.Error, authorized.StatusCode);

        if (appointment.StatusCode == AppointmentStatusCodes.Cancelled)
            return Result<AppointmentResponse>.Ok(appointment.ToResponse());

        if (appointment.StatusCode != AppointmentStatusCodes.Scheduled)
        {
            return Result<AppointmentResponse>.Fail(
                "Only scheduled appointments can be cancelled.",
                HttpStatusCode.Conflict);
        }

        appointment.Cancel(DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        const string title = "Appointment cancelled";
        var message = $"{appointment.Title} was cancelled.";

        await publishEndpoint.Publish(
            new CreateNotificationEvent(
                appointment.PatientId,
                AppointmentNotifications.TypeCode,
                AppointmentNotifications.SeverityCode,
                title,
                message,
                Guid.NewGuid(),
                RelatedEntityType: AppointmentNotifications.RelatedEntityType,
                RelatedEntityId: appointment.Id,
                HealthInstitutionId: appointment.HealthInstitutionId),
            cancellationToken);

        await publishEndpoint.Publish(
            new CreateNotificationEvent(
                appointment.DoctorId,
                AppointmentNotifications.TypeCode,
                AppointmentNotifications.SeverityCode,
                title,
                message,
                Guid.NewGuid(),
                RelatedEntityType: AppointmentNotifications.RelatedEntityType,
                RelatedEntityId: appointment.Id,
                HealthInstitutionId: appointment.HealthInstitutionId),
            cancellationToken);

        logger.LogInformation(
            "Appointment cancelled. AppointmentId={AppointmentId}",
            appointment.Id);

        return Result<AppointmentResponse>.Ok(appointment.ToResponse());
    }
}
