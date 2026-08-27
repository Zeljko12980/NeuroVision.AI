namespace IdentityService.Application.Commands.User;

public record DeleteUserCommand(Guid UserId) : ICommand<Result>;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }
}

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, Result>
{
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(
        IUserService userService,
        ICurrentUser currentUser,
        IPublishEndpoint publishEndpoint,
        ILogger<DeleteUserCommandHandler> logger)
    {
        _userService = userService;
        _currentUser = currentUser;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == command.UserId)
        {
            _logger.LogWarning("Delete user rejected. Cannot delete own account. UserId={UserId}", command.UserId);
            return Result.Fail("Cannot delete your own account.", HttpStatusCode.Forbidden);
        }

        _logger.LogInformation("Delete user started. UserId={UserId}", command.UserId);

        var result = await _userService.DeleteUserAsync(command.UserId, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogWarning(
                "Delete user failed. UserId={UserId}, Error={Error}",
                command.UserId,
                result.Error);
            return result;
        }

        await _publishEndpoint.Publish(new DeleteUserEvent(command.UserId), cancellationToken);

        _logger.LogInformation("User deleted. UserId={UserId}", command.UserId);
        return Result.NoContent();
    }
}
