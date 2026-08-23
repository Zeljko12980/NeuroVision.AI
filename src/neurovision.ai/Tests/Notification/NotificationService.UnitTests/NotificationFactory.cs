namespace NotificationService.UnitTests;

internal static class NotificationFactory
{
    public static readonly Guid DefaultId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid RecipientId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly DateTime CreatedAt = new(2026, 8, 23, 12, 0, 0, DateTimeKind.Utc);

    public static Notification Create(
        Guid? id = null,
        Guid? recipientUserId = null,
        string typeCode = NotificationTypeCodes.System,
        string severityCode = NotificationSeverityCodes.Info,
        string title = "Welcome to NeuroVision",
        string message = "Your account is ready.",
        DateTime? createdAt = null,
        Guid? sourceEventId = null)
    {
        return Notification.Create(
            id ?? DefaultId,
            recipientUserId ?? RecipientId,
            typeCode,
            severityCode,
            title,
            message,
            createdAt ?? CreatedAt,
            sourceEventId: sourceEventId);
    }
}
