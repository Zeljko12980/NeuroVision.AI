namespace NotificationService.Application.Common.Mappings;

public static class NotificationMappings
{
    public static NotificationResponse ToResponse(this Notification entity) =>
        new()
        {
            Id = entity.Id,
            RecipientUserId = entity.RecipientUserId,
            TypeCode = entity.TypeCode,
            SeverityCode = entity.SeverityCode,
            Title = entity.Title,
            Message = entity.Message,
            Payload = entity.Payload,
            RelatedEntityType = entity.RelatedEntityType,
            RelatedEntityId = entity.RelatedEntityId,
            HealthInstitutionId = entity.HealthInstitutionId,
            CreatedAt = entity.CreatedAt,
            ReadAt = entity.ReadAt,
            IsRead = entity.ReadAt.HasValue
        };
}
