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
            .MinimumLength(6);
    }
}

public class SetPasswordCommandHandler : ICommandHandler<SetPasswordCommand, Result>
{
    private readonly IIdentityService _identityService;

    public SetPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(SetPasswordCommand command, CancellationToken cancellationToken)
    {
        var success = await _identityService.ResetPasswordAsync(
            command.Email,
            command.Token,
            command.Password,
            cancellationToken);

        if (!success)
            return Result.Fail("Invalid token or user not found.");

        return Result.Ok();
    }
}
