namespace IdentityService.Application.Commands.User;

public record LockUserCommand(Guid UserId) : ICommand<Result>;

public class LockUserCommandValidator : AbstractValidator<LockUserCommand>
{
    public LockUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }
}

public class LockUserCommandHandler : ICommandHandler<LockUserCommand, Result>
{
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<LockUserCommandHandler> _logger;

    public LockUserCommandHandler(
        IUserService userService,
        ICurrentUser currentUser,
        ILogger<LockUserCommandHandler> logger)
    {
        _userService = userService;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result> Handle(LockUserCommand command, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == command.UserId)
        {
            _logger.LogWarning("Lock user rejected. Cannot lock own account. UserId={UserId}", command.UserId);
            return Result.Fail("Cannot lock your own account.", HttpStatusCode.Forbidden);
        }

        _logger.LogInformation("Lock user started. UserId={UserId}", command.UserId);

        var result = await _userService.LockAsync(command.UserId, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogWarning(
                "Lock user failed. UserId={UserId}, Error={Error}",
                command.UserId,
                result.Error);
            return result;
        }

        _logger.LogInformation("User locked. UserId={UserId}", command.UserId);
        return Result.Ok();
    }
}
