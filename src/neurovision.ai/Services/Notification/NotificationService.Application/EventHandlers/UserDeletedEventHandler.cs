namespace NotificationService.Application.EventHandlers;

public class UserDeletedEventHandler : IConsumer<DeleteUserEvent>
{
    private readonly INotificationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<UserDeletedEventHandler> logger;

    public UserDeletedEventHandler(
        INotificationWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<UserDeletedEventHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<DeleteUserEvent> context)
    {
        var userId = context.Message.UserId;

        logger.LogInformation("DeleteUserEvent received for notifications. UserId={UserId}", userId);

        await writes.RemoveForUserAsync(userId, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}
