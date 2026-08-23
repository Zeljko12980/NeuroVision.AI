using NotificationService.Application.Common.Interfaces;

namespace NotificationService.Infrastructure.Persistence;

internal sealed class NotificationWriteStore : INotificationWriteStore
{
    private readonly NotificationDbContext context;

    public NotificationWriteStore(NotificationDbContext context)
    {
        this.context = context;
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await context.Notifications.AddAsync(notification, cancellationToken);
    }

    public async Task AddPreferencesAsync(
        IEnumerable<NotificationPreference> preferences,
        CancellationToken cancellationToken = default)
    {
        await context.NotificationPreferences.AddRangeAsync(preferences, cancellationToken);
    }

    public Task<Notification?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Notifications.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    }

    public Task<Notification?> FindBySourceEventIdAsync(
        Guid sourceEventId,
        CancellationToken cancellationToken = default)
    {
        return context.Notifications.FirstOrDefaultAsync(
            item => item.SourceEventId == sourceEventId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetInboxAsync(
        Guid recipientUserId,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await context.Notifications
            .AsNoTracking()
            .Where(item => item.RecipientUserId == recipientUserId)
            .OrderByDescending(item => item.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetUnreadAsync(
        Guid recipientUserId,
        CancellationToken cancellationToken = default)
    {
        return await context.Notifications
            .Where(item => item.RecipientUserId == recipientUserId && item.ReadAt == null)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountUnreadAsync(
        Guid recipientUserId,
        CancellationToken cancellationToken = default)
    {
        return context.Notifications.CountAsync(
            item => item.RecipientUserId == recipientUserId && item.ReadAt == null,
            cancellationToken);
    }

    public Task<bool> HasPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return context.NotificationPreferences.AnyAsync(item => item.UserId == userId, cancellationToken);
    }

    public async Task RemoveForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var notifications = await context.Notifications
            .Where(item => item.RecipientUserId == userId)
            .ToListAsync(cancellationToken);
        context.Notifications.RemoveRange(notifications);

        var preferences = await context.NotificationPreferences
            .Where(item => item.UserId == userId)
            .ToListAsync(cancellationToken);
        context.NotificationPreferences.RemoveRange(preferences);
    }
}
