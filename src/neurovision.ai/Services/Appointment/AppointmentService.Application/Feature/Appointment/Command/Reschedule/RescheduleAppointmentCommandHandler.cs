namespace AppointmentService.Application.Feature.Appointment.Command.Reschedule;

public sealed class RescheduleAppointmentCommandHandler
    : ICommandHandler<RescheduleAppointmentCommand, Result<AppointmentResponse>>
{
    private readonly IAppointmentWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly IPublishEndpoint publishEndpoint;
    private readonly ILogger<RescheduleAppointmentCommandHandler> logger;

    public RescheduleAppointmentCommandHandler(
        IAppointmentWriteStore writes,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint,
        ILogger<RescheduleAppointmentCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.publishEndpoint = publishEndpoint;
        this.logger = logger;
    }

    public async Task<Result<AppointmentResponse>> Handle(
        RescheduleAppointmentCommand command,
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

        if (appointment.StatusCode != AppointmentStatusCodes.Scheduled)
        {
            return Result<AppointmentResponse>.Fail(
                "Only scheduled appointments can be rescheduled.",
                HttpStatusCode.Conflict);
        }

        if (await writes.HasOverlapAsync(
            appointment.DoctorId,
            command.Request.StartsAt,
            command.Request.EndsAt,
            appointment.Id,
            cancellationToken))
        {
            return Result<AppointmentResponse>.Fail(
                "The doctor already has an appointment in this time range.",
                HttpStatusCode.Conflict);
        }

        appointment.Reschedule(
            command.Request.StartsAt,
            command.Request.EndsAt,
            command.Request.Title,
            command.Request.Notes);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await PublishNotificationsAsync(appointment, cancellationToken);

        logger.LogInformation(
            "Appointment rescheduled. AppointmentId={AppointmentId}",
            appointment.Id);

        return Result<AppointmentResponse>.Ok(appointment.ToResponse());
    }

    private async Task PublishNotificationsAsync(
        global::AppointmentService.Domain.Entities.Appointment appointment,
        CancellationToken cancellationToken)
    {
        const string title = "Appointment rescheduled";
        var message = $"{appointment.Title} was moved to a new time.";

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
    }
}
