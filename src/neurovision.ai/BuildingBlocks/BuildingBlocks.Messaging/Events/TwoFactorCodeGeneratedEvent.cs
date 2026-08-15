namespace BuildingBlocks.Messaging.Events
{
    public record TwoFactorCodeGeneratedEvent(
    string Email,
    string Code,
    string FullName
    ) : IntegrationEvent;
}
