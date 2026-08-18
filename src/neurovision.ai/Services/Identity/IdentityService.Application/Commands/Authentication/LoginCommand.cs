namespace IdentityService.Application.Commands.Authentication;

public record LoginCommand(LoginRequest LoginRequest) : ICommand<Result<AuthResponse>>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.LoginRequest.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.LoginRequest.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IIdentityService identityService,
        IPublishEndpoint publishEndpoint,
        ILogger<LoginCommandHandler> logger)
    {
        _identityService = identityService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.LoginRequest.Email;

        _logger.LogInformation("Login started. Email={Email}", email);

        var signInStatus = await _identityService.SignInAsync(
            email,
            request.LoginRequest.Password,
            cancellationToken);

        if (signInStatus == SignInStatus.LockedOut)
        {
            _logger.LogWarning("Login failed. Account locked. Email={Email}", email);
            return Result<AuthResponse>.Fail(
                "Account is locked. Try again later.",
                HttpStatusCode.Locked);
        }

        if (signInStatus == SignInStatus.NotAllowed)
        {
            _logger.LogWarning("Login failed. Email not confirmed. Email={Email}", email);
            return Result<AuthResponse>.Fail(
                "Email is not confirmed.",
                HttpStatusCode.Forbidden);
        }

        if (signInStatus != SignInStatus.Succeeded)
        {
            _logger.LogWarning("Login failed. Invalid credentials. Email={Email}", email);
            return Result<AuthResponse>.Fail(
                "Invalid credentials.",
                HttpStatusCode.Unauthorized);
        }

        var code = await _identityService.GenerateTwoFactorCodeAsync(email, cancellationToken);

        if (string.IsNullOrWhiteSpace(code))
        {
            _logger.LogError("Failed to generate 2FA code. Email={Email}", email);

            return Result<AuthResponse>.Fail(
                "Unable to generate two-factor authentication code.",
                HttpStatusCode.InternalServerError);
        }

        var userName = await _identityService.GetUserNameByEmailAsync(email, cancellationToken) ?? email;

        await _publishEndpoint.Publish(
            new TwoFactorCodeGeneratedEvent(email, code, userName),
            cancellationToken);

        _logger.LogInformation("2FA code generated and published. Email={Email}", email);

        return Result<AuthResponse>.Ok(new AuthResponse
        {
            Email = email,
            Message = "Two-factor authentication code sent successfully."
        });
    }
}
