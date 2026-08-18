namespace IdentityService.Application.Commands.Authentication;

public record ForgotPasswordCommand(string Email) : ICommand<Result<AuthResponse>>;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, Result<AuthResponse>>
{
    private static readonly AuthResponse GenericResponse = new()
    {
        Message = "If an account exists for this email, a password reset link has been sent."
    };

    private readonly IIdentityService _identityService;
    private readonly IFrontendLinkService _frontendLinkService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IIdentityService identityService,
        IFrontendLinkService frontendLinkService,
        IPublishEndpoint publishEndpoint,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _identityService = identityService;
        _frontendLinkService = frontendLinkService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> Handle(
        ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var email = command.Email;
        _logger.LogInformation("Forgot password requested. Email={Email}", email);

        var token = await _identityService.GeneratePasswordResetTokenAsync(email, cancellationToken);
        if (string.IsNullOrEmpty(token))
            return Result<AuthResponse>.Ok(GenericResponse);

        var linkResult = _frontendLinkService.BuildSetPasswordLink(email, token);
        if (!linkResult.IsSuccess)
        {
            _logger.LogError(
                "Forgot password link build failed. Email={Email}, Error={Error}",
                email,
                linkResult.Error);
            return Result<AuthResponse>.Ok(GenericResponse);
        }

        await _publishEndpoint.Publish(new ForgotPasswordEvent(email, linkResult.Value), cancellationToken);
        _logger.LogInformation("Forgot password event published. Email={Email}", email);

        return Result<AuthResponse>.Ok(GenericResponse);
    }
}
