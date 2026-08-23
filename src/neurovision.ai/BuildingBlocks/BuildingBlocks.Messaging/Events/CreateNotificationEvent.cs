namespace BuildingBlocks.Messaging.Events;

public record CreateNotificationEvent(
    Guid RecipientUserId,
    string TypeCode,
    string SeverityCode,
    string Title,
    string Message,
    Guid SourceEventId,
    string? Payload = null,
    string? RelatedEntityType = null,
    Guid? RelatedEntityId = null,
    int? HealthInstitutionId = null) : IntegrationEvent;
