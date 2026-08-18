namespace IdentityService.Application.Commands.Authentication;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : ICommand<Result>;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("New password must be different from the current password.");
    }
}

public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, Result>
{
    private readonly ICurrentUser _currentUser;
    private readonly IIdentityService _identityService;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        ICurrentUser currentUser,
        IIdentityService identityService,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _currentUser = currentUser;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is not Guid userId)
        {
            _logger.LogWarning("Change password failed. User is not authenticated.");
            return Result.Fail("Unauthorized.", HttpStatusCode.Unauthorized);
        }

        _logger.LogInformation("Change password started. UserId={UserId}", userId);

        var result = await _identityService.ChangePasswordAsync(
            userId,
            command.CurrentPassword,
            command.NewPassword,
            cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning(
                "Change password failed. UserId={UserId}, Error={Error}",
                userId,
                result.Error);
            return result;
        }

        _logger.LogInformation("Password changed. UserId={UserId}", userId);
        return Result.Ok();
    }
}
