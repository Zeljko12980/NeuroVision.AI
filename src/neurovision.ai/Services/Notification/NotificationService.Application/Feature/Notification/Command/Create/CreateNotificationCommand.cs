namespace NotificationService.Application.Feature.Notification.Command.Create;

public sealed record CreateNotificationCommand(
    Guid RecipientUserId,
    string TypeCode,
    string SeverityCode,
    string Title,
    string Message,
    Guid? SourceEventId = null,
    string? Payload = null,
    string? RelatedEntityType = null,
    Guid? RelatedEntityId = null,
    int? HealthInstitutionId = null) : ICommand<Result<NotificationResponse>>;

public sealed class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationCommandValidator()
    {
        RuleFor(x => x.RecipientUserId).NotEmpty();
        RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.SeverityCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(512);
        RuleFor(x => x.RelatedEntityType).MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.RelatedEntityType));
        RuleFor(x => x.RelatedEntityId)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.RelatedEntityType));
    }
}
