namespace NotificationService.Application.Feature.Notification.Command.Create;

public sealed class CreateNotificationCommandHandler
    : ICommandHandler<CreateNotificationCommand, Result<NotificationResponse>>
{
    private readonly INotificationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly INotificationRealtimePublisher realtime;
    private readonly ILogger<CreateNotificationCommandHandler> logger;

    public CreateNotificationCommandHandler(
        INotificationWriteStore writes,
        IUnitOfWork unitOfWork,
        INotificationRealtimePublisher realtime,
        ILogger<CreateNotificationCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.realtime = realtime;
        this.logger = logger;
    }

    public async Task<Result<NotificationResponse>> Handle(
        CreateNotificationCommand command,
        CancellationToken cancellationToken)
    {
        if (command.SourceEventId is Guid sourceEventId)
        {
            var existing = await writes.FindBySourceEventIdAsync(sourceEventId, cancellationToken);
            if (existing is not null)
            {
                logger.LogInformation(
                    "Skipping duplicate notification. SourceEventId={SourceEventId}, NotificationId={NotificationId}",
                    sourceEventId,
                    existing.Id);
                return Result<NotificationResponse>.Ok(existing.ToResponse());
            }
        }

        var notification = global::NotificationService.Domain.Entities.Notification.Create(
            Guid.NewGuid(),
            command.RecipientUserId,
            command.TypeCode,
            command.SeverityCode,
            command.Title,
            command.Message,
            DateTime.UtcNow,
            command.Payload,
            command.RelatedEntityType,
            command.RelatedEntityId,
            command.HealthInstitutionId,
            command.SourceEventId);

        await writes.AddAsync(notification, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = notification.ToResponse();
        try
        {
            await realtime.PublishCreatedAsync(response, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Failed to push notification over SignalR. NotificationId={NotificationId}",
                notification.Id);
        }

        logger.LogInformation(
            "Notification created. NotificationId={NotificationId}, RecipientUserId={RecipientUserId}, Type={TypeCode}",
            notification.Id,
            notification.RecipientUserId,
            notification.TypeCode);

        return Result<NotificationResponse>.Created(response);
    }
}
