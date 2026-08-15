namespace BuildingBlocks.Messaging.Events
{
    public record ConfirmEmailEvent
        (Guid UserId, string Email, string token)
        : IntegrationEvent;
}
