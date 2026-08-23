namespace NotificationService.Application.Common.Response;

public class NotificationResponse
{
    public Guid Id { get; set; }
    public Guid RecipientUserId { get; set; }
    public string TypeCode { get; set; } = null!;
    public string SeverityCode { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? Payload { get; set; }
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public int? HealthInstitutionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsRead { get; set; }
}

public class NotificationInboxResponse
{
    public IReadOnlyList<NotificationResponse> Items { get; set; } = [];
    public int UnreadCount { get; set; }
}
