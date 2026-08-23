namespace NotificationService.Application.Common.Interfaces;

public interface INotificationWriteStore
{
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);

    Task AddPreferencesAsync(
        IEnumerable<NotificationPreference> preferences,
        CancellationToken cancellationToken = default);

    Task<Notification?> FindAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Notification?> FindBySourceEventIdAsync(
        Guid sourceEventId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Notification>> GetInboxAsync(
        Guid recipientUserId,
        int take,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Notification>> GetUnreadAsync(
        Guid recipientUserId,
        CancellationToken cancellationToken = default);

    Task<int> CountUnreadAsync(
        Guid recipientUserId,
        CancellationToken cancellationToken = default);

    Task<bool> HasPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task RemoveForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
