namespace NotificationService.Application.EventHandlers;

public class CreateNotificationEventHandler : IConsumer<CreateNotificationEvent>
{
    private readonly ISender sender;
    private readonly ILogger<CreateNotificationEventHandler> logger;

    public CreateNotificationEventHandler(
        ISender sender,
        ILogger<CreateNotificationEventHandler> logger)
    {
        this.sender = sender;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<CreateNotificationEvent> context)
    {
        var message = context.Message;

        logger.LogInformation(
            "CreateNotificationEvent received. RecipientUserId={RecipientUserId}, Type={TypeCode}, SourceEventId={SourceEventId}",
            message.RecipientUserId,
            message.TypeCode,
            message.SourceEventId);

        var result = await sender.Send(
            new Feature.Notification.Command.Create.CreateNotificationCommand(
                message.RecipientUserId,
                message.TypeCode,
                message.SeverityCode,
                message.Title,
                message.Message,
                message.SourceEventId,
                message.Payload,
                message.RelatedEntityType,
                message.RelatedEntityId,
                message.HealthInstitutionId),
            context.CancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError(
                "CreateNotificationEvent failed. RecipientUserId={RecipientUserId}, Error={Error}",
                message.RecipientUserId,
                result.Error);
            throw new InvalidOperationException(result.Error);
        }
    }
}
