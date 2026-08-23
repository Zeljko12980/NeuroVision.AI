namespace NotificationService.Domain.Entities;

public class NotificationPreference
{
    public Guid UserId { get; private set; }
    public string TypeCode { get; private set; } = null!;
    public string ChannelCode { get; private set; } = null!;
    public bool Enabled { get; private set; }

    public NotificationType Type { get; private set; } = null!;
    public NotificationChannel Channel { get; private set; } = null!;

    private NotificationPreference()
    {
    }

    public static NotificationPreference Create(
        Guid userId,
        string typeCode,
        string channelCode,
        bool enabled = true)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id is required.", nameof(userId));

        return new NotificationPreference
        {
            UserId = userId,
            TypeCode = Guard.Code(typeCode, nameof(typeCode)),
            ChannelCode = Guard.Code(channelCode, nameof(channelCode)),
            Enabled = enabled
        };
    }

    public void SetEnabled(bool enabled)
    {
        Enabled = enabled;
    }
}
