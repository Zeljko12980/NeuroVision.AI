namespace NotificationService.Application.EventHandlers;

public class UserCreatedEventHandler : IConsumer<CreateUserEvent>
{
    private readonly INotificationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ISender sender;
    private readonly ILogger<UserCreatedEventHandler> logger;

    public UserCreatedEventHandler(
        INotificationWriteStore writes,
        IUnitOfWork unitOfWork,
        ISender sender,
        ILogger<UserCreatedEventHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.sender = sender;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<CreateUserEvent> context)
    {
        var message = context.Message;
        var cancellationToken = context.CancellationToken;

        logger.LogInformation(
            "CreateUserEvent received for notifications. UserId={UserId}, RoleName={RoleName}",
            message.UserId,
            message.RoleName);

        if (!await writes.HasPreferencesAsync(message.UserId, cancellationToken))
        {
            var preferences =
                from type in NotificationTypeCodes.All
                from channel in NotificationChannelCodes.All
                select NotificationPreference.Create(message.UserId, type, channel);

            await writes.AddPreferencesAsync(preferences, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var result = await sender.Send(
            new Feature.Notification.Command.Create.CreateNotificationCommand(
                message.UserId,
                NotificationTypeCodes.System,
                NotificationSeverityCodes.Info,
                "Welcome to NeuroVision",
                $"Your {message.RoleName.ToLowerInvariant()} account is ready.",
                message.UserId),
            cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError(
                "Welcome notification failed. UserId={UserId}, Error={Error}",
                message.UserId,
                result.Error);
            throw new InvalidOperationException(result.Error);
        }
    }
}
