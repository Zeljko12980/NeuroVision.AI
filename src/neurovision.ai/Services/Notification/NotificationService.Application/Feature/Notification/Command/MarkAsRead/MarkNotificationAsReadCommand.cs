namespace NotificationService.Application.Feature.Notification.Command.MarkAsRead;

public sealed record MarkNotificationAsReadCommand(Guid Id, Guid RecipientUserId)
    : ICommand<Result<NotificationResponse>>;

public sealed class MarkNotificationAsReadCommandValidator : AbstractValidator<MarkNotificationAsReadCommand>
{
    public MarkNotificationAsReadCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.RecipientUserId).NotEmpty();
    }
}
