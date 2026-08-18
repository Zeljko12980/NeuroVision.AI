namespace IdentityService.Application.Commands.Authentication;

public record Resend2FACommand(Resend2FARequest Resend2FARequest)
    : ICommand<Result<Confirm2FAResponse>>;

public class Resend2FACommandValidator : AbstractValidator<Resend2FACommand>
{
    public Resend2FACommandValidator()
    {
        RuleFor(x => x.Resend2FARequest.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}

public class Resend2FACommandHandler
    : ICommandHandler<Resend2FACommand, Result<Confirm2FAResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<Resend2FACommandHandler> _logger;

    public Resend2FACommandHandler(
        IIdentityService identityService,
        IPublishEndpoint publishEndpoint,
        ILogger<Resend2FACommandHandler> logger)
    {
        _identityService = identityService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<Result<Confirm2FAResponse>> Handle(
        Resend2FACommand command,
        CancellationToken cancellationToken)
    {
        var email = command.Resend2FARequest.Email;
        _logger.LogInformation("Resend 2FA started. Email={Email}", email);

        var code = await _identityService.GenerateTwoFactorCodeAsync(email, cancellationToken);

        if (code is null)
        {
            _logger.LogWarning("Resend 2FA failed. Unable to generate code. Email={Email}", email);
            return Result<Confirm2FAResponse>.Fail("Unable to generate 2FA code.");
        }

        var userName = await _identityService.GetUserNameByEmailAsync(email, cancellationToken) ?? email;

        await _publishEndpoint.Publish(
            new TwoFactorCodeGeneratedEvent(email, code, userName),
            cancellationToken);

        _logger.LogInformation("Resend 2FA succeeded. Email={Email}", email);

        return Result<Confirm2FAResponse>.Ok(new Confirm2FAResponse
        {
            Message = "New two-factor code sent."
        });
    }
}
