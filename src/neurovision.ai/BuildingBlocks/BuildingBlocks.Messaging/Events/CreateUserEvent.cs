namespace BuildingBlocks.Messaging.Events
{
    public record CreateUserEvent(
           Guid UserId,
           string Username,
           string Email,
           string RoleName
       ) : IntegrationEvent;
}
