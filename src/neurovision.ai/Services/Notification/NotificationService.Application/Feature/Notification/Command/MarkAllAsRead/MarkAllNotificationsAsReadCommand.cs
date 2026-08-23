namespace NotificationService.Application.Feature.Notification.Command.MarkAllAsRead;

public sealed record MarkAllNotificationsAsReadCommand(Guid RecipientUserId)
    : ICommand<Result<int>>;

public sealed class MarkAllNotificationsAsReadCommandValidator : AbstractValidator<MarkAllNotificationsAsReadCommand>
{
    public MarkAllNotificationsAsReadCommandValidator()
    {
        RuleFor(x => x.RecipientUserId).NotEmpty();
    }
}
