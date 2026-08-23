namespace NotificationService.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public Guid RecipientUserId { get; private set; }
    public string TypeCode { get; private set; } = null!;
    public string SeverityCode { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public string? Payload { get; private set; }
    public string? RelatedEntityType { get; private set; }
    public Guid? RelatedEntityId { get; private set; }
    public int? HealthInstitutionId { get; private set; }
    public Guid? SourceEventId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public NotificationType Type { get; private set; } = null!;
    public NotificationSeverity Severity { get; private set; } = null!;

    private Notification()
    {
    }

    public static Notification Create(
        Guid id,
        Guid recipientUserId,
        string typeCode,
        string severityCode,
        string title,
        string message,
        DateTime createdAt,
        string? payload = null,
        string? relatedEntityType = null,
        Guid? relatedEntityId = null,
        int? healthInstitutionId = null,
        Guid? sourceEventId = null)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Notification id is required.", nameof(id));

        if (recipientUserId == Guid.Empty)
            throw new ArgumentException("Recipient user id is required.", nameof(recipientUserId));

        if (relatedEntityId.HasValue && relatedEntityId.Value == Guid.Empty)
            throw new ArgumentException("Related entity id cannot be empty.", nameof(relatedEntityId));

        if (sourceEventId.HasValue && sourceEventId.Value == Guid.Empty)
            throw new ArgumentException("Source event id cannot be empty.", nameof(sourceEventId));

        var normalizedRelatedEntityType = string.IsNullOrWhiteSpace(relatedEntityType)
            ? null
            : Guard.MaxLength(Guard.NotEmpty(relatedEntityType, nameof(relatedEntityType)), nameof(relatedEntityType), 50);

        if (relatedEntityId.HasValue && normalizedRelatedEntityType is null)
            throw new ArgumentException("Related entity type is required when related entity id is set.", nameof(relatedEntityType));

        return new Notification
        {
            Id = id,
            RecipientUserId = recipientUserId,
            TypeCode = Guard.Code(typeCode, nameof(typeCode)),
            SeverityCode = Guard.Code(severityCode, nameof(severityCode)),
            Title = Guard.MaxLength(Guard.NotEmpty(title, nameof(title)), nameof(title), 120),
            Message = Guard.MaxLength(Guard.NotEmpty(message, nameof(message)), nameof(message), 512),
            Payload = payload,
            RelatedEntityType = normalizedRelatedEntityType,
            RelatedEntityId = relatedEntityId,
            HealthInstitutionId = healthInstitutionId,
            SourceEventId = sourceEventId,
            CreatedAt = createdAt
        };
    }

    public void MarkAsRead(DateTime readAt)
    {
        if (ReadAt.HasValue)
            return;

        ReadAt = readAt;
    }
}
