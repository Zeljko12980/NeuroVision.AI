namespace AppointmentService.Application.Feature.Appointment.Command.Create;

public sealed class CreateAppointmentCommandHandler
    : ICommandHandler<CreateAppointmentCommand, Result<AppointmentResponse>>
{
    private readonly IAppointmentWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly IPublishEndpoint publishEndpoint;
    private readonly ILogger<CreateAppointmentCommandHandler> logger;

    public CreateAppointmentCommandHandler(
        IAppointmentWriteStore writes,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint,
        ILogger<CreateAppointmentCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.publishEndpoint = publishEndpoint;
        this.logger = logger;
    }

    public async Task<Result<AppointmentResponse>> Handle(
        CreateAppointmentCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var authorized = AppointmentAccess.AuthorizeCreate(command.Actor, request);
        if (authorized.IsFailure)
            return Result<AppointmentResponse>.Fail(authorized.Error, authorized.StatusCode);

        if (!await writes.TypeExistsAsync(request.TypeCode, cancellationToken))
        {
            return Result<AppointmentResponse>.Fail(
                $"Appointment type '{request.TypeCode}' was not found.",
                HttpStatusCode.NotFound);
        }

        if (await writes.HasOverlapAsync(
            request.DoctorId,
            request.StartsAt,
            request.EndsAt,
            excludeId: null,
            cancellationToken))
        {
            return Result<AppointmentResponse>.Fail(
                "The doctor already has an appointment in this time range.",
                HttpStatusCode.Conflict);
        }

        var appointment = global::AppointmentService.Domain.Entities.Appointment.Create(
            Guid.NewGuid(),
            request.PatientId,
            request.DoctorId,
            request.TypeCode,
            request.StartsAt,
            request.EndsAt,
            request.Title,
            DateTime.UtcNow,
            request.Notes,
            request.HealthInstitutionId);

        await writes.AddAsync(appointment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await PublishNotificationsAsync(
            appointment,
            "Appointment scheduled",
            $"{appointment.Title} is scheduled.",
            cancellationToken);

        logger.LogInformation(
            "Appointment created. AppointmentId={AppointmentId}, DoctorId={DoctorId}, PatientId={PatientId}",
            appointment.Id,
            appointment.DoctorId,
            appointment.PatientId);

        return Result<AppointmentResponse>.Created(appointment.ToResponse());
    }

    private async Task PublishNotificationsAsync(
        global::AppointmentService.Domain.Entities.Appointment appointment,
        string title,
        string message,
        CancellationToken cancellationToken)
    {
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
