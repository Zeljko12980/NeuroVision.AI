using Microsoft.AspNetCore.SignalR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Common.Response;

namespace NotificationService.API.Hubs;

public sealed class SignalRNotificationRealtimePublisher(IHubContext<NotificationHub> hub)
    : INotificationRealtimePublisher
{
    public const string NotificationCreated = "NotificationCreated";

    public Task PublishCreatedAsync(
        NotificationResponse notification,
        CancellationToken cancellationToken = default) =>
        hub.Clients
            .User(notification.RecipientUserId.ToString())
            .SendAsync(NotificationCreated, notification, cancellationToken);
}
