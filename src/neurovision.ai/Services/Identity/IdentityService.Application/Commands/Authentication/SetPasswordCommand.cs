namespace IdentityService.Application.Commands.Authentication;

public record SetPasswordCommand(
    string Email,
    string Token,
    string Password
) : ICommand<Result>;

public class SetPasswordCommandValidator : AbstractValidator<SetPasswordCommand>
{
    public SetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}

public class SetPasswordCommandHandler : ICommandHandler<SetPasswordCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<SetPasswordCommandHandler> _logger;

    public SetPasswordCommandHandler(
        IIdentityService identityService,
        ILogger<SetPasswordCommandHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result> Handle(SetPasswordCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Set password started. Email={Email}", command.Email);

        var success = await _identityService.ResetPasswordAsync(
            command.Email,
            command.Token,
            command.Password,
            cancellationToken);

        if (!success)
        {
            _logger.LogWarning("Set password failed. Email={Email}", command.Email);
            return Result.Fail("Invalid token or user not found.");
        }

        _logger.LogInformation("Password set successfully. Email={Email}", command.Email);
        return Result.Ok();
    }
}
