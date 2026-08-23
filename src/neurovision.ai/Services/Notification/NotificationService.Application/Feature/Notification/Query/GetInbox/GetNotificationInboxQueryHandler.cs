namespace NotificationService.Application.Feature.Notification.Query.GetInbox;

public sealed class GetNotificationInboxQueryHandler
    : IQueryHandler<GetNotificationInboxQuery, Result<NotificationInboxResponse>>
{
    private readonly INotificationWriteStore writes;
    private readonly ILogger<GetNotificationInboxQueryHandler> logger;

    public GetNotificationInboxQueryHandler(
        INotificationWriteStore writes,
        ILogger<GetNotificationInboxQueryHandler> logger)
    {
        this.writes = writes;
        this.logger = logger;
    }

    public async Task<Result<NotificationInboxResponse>> Handle(
        GetNotificationInboxQuery query,
        CancellationToken cancellationToken)
    {
        if (query.RecipientUserId == Guid.Empty)
            return Result<NotificationInboxResponse>.Fail(
                "Recipient user id is required.",
                HttpStatusCode.BadRequest);

        var take = Math.Clamp(query.Take, 1, 100);
        var items = await writes.GetInboxAsync(query.RecipientUserId, take, cancellationToken);
        var unreadCount = await writes.CountUnreadAsync(query.RecipientUserId, cancellationToken);

        logger.LogInformation(
            "Get inbox succeeded. RecipientUserId={RecipientUserId}, Count={Count}, Unread={Unread}",
            query.RecipientUserId,
            items.Count,
            unreadCount);

        return Result<NotificationInboxResponse>.Ok(new NotificationInboxResponse
        {
            Items = items.Select(item => item.ToResponse()).ToList(),
            UnreadCount = unreadCount
        });
    }
}
