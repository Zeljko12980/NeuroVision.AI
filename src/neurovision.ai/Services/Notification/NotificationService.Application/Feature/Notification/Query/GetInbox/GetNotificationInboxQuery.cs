namespace NotificationService.Application.Feature.Notification.Query.GetInbox;

public sealed record GetNotificationInboxQuery(Guid RecipientUserId, int Take = 20)
    : IQuery<Result<NotificationInboxResponse>>;
