namespace NotificationService.Application.Feature.Notification.Command.MarkAsRead;

public sealed class MarkNotificationAsReadCommandHandler
    : ICommandHandler<MarkNotificationAsReadCommand, Result<NotificationResponse>>
{
    private readonly INotificationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<MarkNotificationAsReadCommandHandler> logger;

    public MarkNotificationAsReadCommandHandler(
        INotificationWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<MarkNotificationAsReadCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result<NotificationResponse>> Handle(
        MarkNotificationAsReadCommand command,
        CancellationToken cancellationToken)
    {
        var notification = await writes.FindAsync(command.Id, cancellationToken);
        if (notification is null || notification.RecipientUserId != command.RecipientUserId)
        {
            logger.LogWarning(
                "Mark notification read failed. NotificationId={NotificationId}, RecipientUserId={RecipientUserId}",
                command.Id,
                command.RecipientUserId);
            return Result<NotificationResponse>.Fail("Notification not found.", HttpStatusCode.NotFound);
        }

        notification.MarkAsRead(DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<NotificationResponse>.Ok(notification.ToResponse());
    }
}
