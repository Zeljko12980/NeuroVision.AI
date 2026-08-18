namespace IdentityService.Application.Commands.User;

public record UnlockUserCommand(Guid UserId) : ICommand<Result>;

public class UnlockUserCommandValidator : AbstractValidator<UnlockUserCommand>
{
    public UnlockUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }
}

public class UnlockUserCommandHandler : ICommandHandler<UnlockUserCommand, Result>
{
    private readonly IUserService _userService;
    private readonly ILogger<UnlockUserCommandHandler> _logger;

    public UnlockUserCommandHandler(
        IUserService userService,
        ILogger<UnlockUserCommandHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<Result> Handle(UnlockUserCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unlock user started. UserId={UserId}", command.UserId);

        var result = await _userService.UnlockAsync(command.UserId, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogWarning(
                "Unlock user failed. UserId={UserId}, Error={Error}",
                command.UserId,
                result.Error);
            return result;
        }

        _logger.LogInformation("User unlocked. UserId={UserId}", command.UserId);
        return Result.Ok();
    }
}
