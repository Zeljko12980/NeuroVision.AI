namespace NotificationService.Domain.Entities;

public class NotificationChannel
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public ICollection<NotificationPreference> Preferences { get; private set; } = new List<NotificationPreference>();

    private NotificationChannel()
    {
    }

    public static NotificationChannel Create(string code, string name, string? description = null)
    {
        return new NotificationChannel
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
