namespace BuildingBlocks.Messaging.Events
{
    public record SetPasswordEvent
         (string Email, string Url)
         : IntegrationEvent;
}
