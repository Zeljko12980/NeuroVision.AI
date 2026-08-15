namespace IdentityService.Application.Commands.Authentication;

public class ConfirmEmailCommand : ICommand<Result<ConfirmEmailResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Confirmation token is required.");
    }
}

public class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand, Result<ConfirmEmailResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IFrontendLinkService _frontendLinkService;
    private readonly IPublishEndpoint _publishEndpoint;

    public ConfirmEmailCommandHandler(
        IIdentityService identityService,
        IFrontendLinkService frontendLinkService,
        IPublishEndpoint publishEndpoint)
    {
        _identityService = identityService;
        _frontendLinkService = frontendLinkService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<ConfirmEmailResponse>> Handle(
        ConfirmEmailCommand command,
        CancellationToken cancellationToken)
    {
        var confirmed = await _identityService.ConfirmEmailAsync(
            command.Email,
            command.Token,
            cancellationToken);

        if (!confirmed)
            return Result<ConfirmEmailResponse>.Fail("Email confirmation failed.");

        var token = await _identityService.GeneratePasswordResetTokenAsync(
            command.Email,
            cancellationToken);

        if (string.IsNullOrEmpty(token))
            return Result<ConfirmEmailResponse>.Fail("User not found or token generation failed.");

        var linkResult = _frontendLinkService.BuildSetPasswordLink(command.Email, token);

        if (!linkResult.IsSuccess)
            return Result<ConfirmEmailResponse>.Fail(linkResult.Error);

        await _publishEndpoint.Publish(new SetPasswordEvent(command.Email, linkResult.Value), cancellationToken);

        return Result<ConfirmEmailResponse>.Ok(new ConfirmEmailResponse
        {
            IsConfirmed = true
        });
    }
}
