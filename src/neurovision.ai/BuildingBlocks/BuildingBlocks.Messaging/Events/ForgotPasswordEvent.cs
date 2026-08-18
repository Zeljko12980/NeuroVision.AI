namespace BuildingBlocks.Messaging.Events;

public record ForgotPasswordEvent(string Email, string Url) : IntegrationEvent;
