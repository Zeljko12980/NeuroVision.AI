using NotificationService.Application.Common.Response;

namespace NotificationService.Application.Common.Interfaces;

public interface INotificationRealtimePublisher
{
    Task PublishCreatedAsync(
        NotificationResponse notification,
        CancellationToken cancellationToken = default);
}
