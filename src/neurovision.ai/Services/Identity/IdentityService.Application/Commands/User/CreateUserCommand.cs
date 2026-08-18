namespace IdentityService.Application.Commands.User;

public class CreateUserCommand : ICommand<Result<UserResponse>>
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().MinimumLength(3);

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();

        RuleFor(x => x.Roles)
            .NotEmpty().WithMessage("At least one role is required.")
            .Must(roles => roles is null || roles.All(role =>
                !string.Equals(role, RoleNames.SuperAdministrator, StringComparison.OrdinalIgnoreCase)))
            .WithMessage("SuperAdministrator cannot be assigned through this endpoint.");
    }
}

public class CreateUserCommandHandler
    : ICommandHandler<CreateUserCommand, Result<UserResponse>>
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IFrontendLinkService _frontendLinkService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserService userService,
        IRoleService roleService,
        IFrontendLinkService frontendLinkService,
        IPublishEndpoint publishEndpoint,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userService = userService;
        _roleService = roleService;
        _frontendLinkService = frontendLinkService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<Result<UserResponse>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
            command.Id = Guid.NewGuid();

        _logger.LogInformation("Create user started. UserId={UserId}, Email={Email}", command.Id, command.Email);

        var userResult = await _userService.CreateAsync(
            command.Id,
            command.UserName,
            command.Email,
            cancellationToken);

        if (!userResult.IsSuccess)
        {
            _logger.LogWarning("Create user failed. UserId={UserId}, Email={Email}, Error={Error}", command.Id, command.Email, userResult.Error);
            return Result<UserResponse>.Fail(userResult.Error);
        }

        var user = userResult.Value;

        var roleResult = await _roleService.AssignRolesAsync(user.Id, command.Roles, cancellationToken);

        if (!roleResult.IsSuccess)
        {
            _logger.LogWarning("Assign roles failed after user create. UserId={UserId}, Error={Error}", user.Id, roleResult.Error);
            return Result<UserResponse>.Fail(roleResult.Error);
        }

        var emailResult = await SendConfirmationEmailAsync(user, cancellationToken);

        if (!emailResult.IsSuccess)
        {
            _logger.LogError("Confirmation email failed after user create. UserId={UserId}, Error={Error}", user.Id, emailResult.Error);
            return Result<UserResponse>.Fail(emailResult.Error);
        }

        _logger.LogInformation("User created successfully. UserId={UserId}, Email={Email}", user.Id, user.Email);

        return Result<UserResponse>.Ok(user.ToResponse());
    }

    private async Task<Result> SendConfirmationEmailAsync(IdentityService.Domain.Entities.User user, CancellationToken cancellationToken)
    {
        var tokenResult = await _userService.GenerateEmailConfirmationTokenAsync(user.Id, cancellationToken);

        if (!tokenResult.IsSuccess)
            return Result.Fail(tokenResult.Error);

        var linkResult = _frontendLinkService.BuildConfirmEmailLink(user.Email, tokenResult.Value);

        if (!linkResult.IsSuccess)
            return Result.Fail(linkResult.Error);

        await _publishEndpoint.Publish(new ConfirmEmailEvent(
            user.Id,
            user.Email,
            linkResult.Value),
            cancellationToken);

        return Result.Ok();
    }
}
