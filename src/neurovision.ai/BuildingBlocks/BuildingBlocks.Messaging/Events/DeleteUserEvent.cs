namespace BuildingBlocks.Messaging.Events
{
    public record DeleteUserEvent(Guid UserId) : IntegrationEvent;
}
