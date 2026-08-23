namespace NotificationService.Domain.Entities;

public class NotificationType
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public ICollection<Notification> Notifications { get; private set; } = new List<Notification>();
    public ICollection<NotificationPreference> Preferences { get; private set; } = new List<NotificationPreference>();

    private NotificationType()
    {
    }

    public static NotificationType Create(string code, string name, string? description = null)
    {
        return new NotificationType
        {
            Code = Guard.Code(code, nameof(code)),
            Name = Guard.NotEmpty(name, nameof(name)),
            Description = description
        };
    }

    public void Update(string name, string? description)
    {
        Name = Guard.NotEmpty(name, nameof(name));
        Description = description;
    }
}
