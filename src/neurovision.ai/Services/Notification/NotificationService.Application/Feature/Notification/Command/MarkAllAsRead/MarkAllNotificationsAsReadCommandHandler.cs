namespace NotificationService.Application.Feature.Notification.Command.MarkAllAsRead;

public sealed class MarkAllNotificationsAsReadCommandHandler
    : ICommandHandler<MarkAllNotificationsAsReadCommand, Result<int>>
{
    private readonly INotificationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<MarkAllNotificationsAsReadCommandHandler> logger;

    public MarkAllNotificationsAsReadCommandHandler(
        INotificationWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<MarkAllNotificationsAsReadCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result<int>> Handle(
        MarkAllNotificationsAsReadCommand command,
        CancellationToken cancellationToken)
    {
        var unread = await writes.GetUnreadAsync(command.RecipientUserId, cancellationToken);
        var now = DateTime.UtcNow;

        foreach (var notification in unread)
            notification.MarkAsRead(now);

        if (unread.Count > 0)
            await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Marked notifications read. RecipientUserId={RecipientUserId}, Count={Count}",
            command.RecipientUserId,
            unread.Count);

        return Result<int>.Ok(unread.Count);
    }
}
