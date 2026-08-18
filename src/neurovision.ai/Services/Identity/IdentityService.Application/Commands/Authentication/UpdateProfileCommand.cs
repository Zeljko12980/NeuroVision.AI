namespace IdentityService.Application.Commands.Authentication;

public record UpdateProfileCommand(string UserName, string? PhoneNumber)
    : ICommand<Result<UserResponse>>;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("User name is required.")
            .MinimumLength(3).WithMessage("User name must be at least 3 characters long.")
            .MaximumLength(256).WithMessage("User name must be at most 256 characters long.")
            .Matches(@"^[a-zA-Z0-9._@+-]+$")
            .WithMessage("User name contains invalid characters.");

        RuleFor(x => x.PhoneNumber)
            .Must(InternationalPhoneNumber.IsValid)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage($"Phone number must be in international format, e.g. {InternationalPhoneNumber.Example}.");
    }
}

public class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, Result<UserResponse>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ILogger<UpdateProfileCommandHandler> _logger;

    public UpdateProfileCommandHandler(
        ICurrentUser currentUser,
        IUserService userService,
        IRoleService roleService,
        ILogger<UpdateProfileCommandHandler> logger)
    {
        _currentUser = currentUser;
        _userService = userService;
        _roleService = roleService;
        _logger = logger;
    }

    public async Task<Result<UserResponse>> Handle(
        UpdateProfileCommand command,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is not Guid userId)
        {
            _logger.LogWarning("Update profile failed. User is not authenticated.");
            return Result<UserResponse>.Fail("Unauthorized.", HttpStatusCode.Unauthorized);
        }

        _logger.LogInformation("Update profile started. UserId={UserId}", userId);

        var result = await _userService.UpdateProfileAsync(
            userId,
            command.UserName,
            command.PhoneNumber,
            cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning(
                "Update profile failed. UserId={UserId}, Error={Error}",
                userId,
                result.Error);
            return Result<UserResponse>.Fail(result.Error, result.StatusCode);
        }

        var response = result.Value.ToResponse();
        var rolesResult = await _roleService.GetUserRolesAsync(userId, cancellationToken);
        if (rolesResult.IsSuccess)
            response.Roles = rolesResult.Value;

        _logger.LogInformation("Profile updated. UserId={UserId}", userId);
        return Result<UserResponse>.Ok(response);
    }
}
