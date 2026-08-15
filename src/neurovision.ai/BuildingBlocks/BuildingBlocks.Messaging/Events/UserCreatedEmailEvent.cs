namespace BuildingBlocks.Messaging.Events
{
    public record UserCreatedEmailEvent(
         Guid UserId,
         string Email,
         string FullName,
         string Username,
         string Password
     ) : IntegrationEvent;
}
