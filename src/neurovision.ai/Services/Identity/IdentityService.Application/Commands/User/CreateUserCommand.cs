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
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID is required.");

        RuleFor(x => x.UserName)
            .NotEmpty().MinimumLength(3);

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();

        RuleFor(x => x.Roles)
            .NotNull();
    }
}

public class CreateUserCommandHandler
    : ICommandHandler<CreateUserCommand, Result<UserResponse>>
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IFrontendLinkService _frontendLinkService;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateUserCommandHandler(
        IUserService userService,
        IRoleService roleService,
        IFrontendLinkService frontendLinkService,
        IPublishEndpoint publishEndpoint)
    {
        _userService = userService;
        _roleService = roleService;
        _frontendLinkService = frontendLinkService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<UserResponse>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var userResult = await _userService.CreateAsync(
            command.Id,
            command.UserName,
            command.Email,
            cancellationToken);

        if (!userResult.IsSuccess)
            return Result<UserResponse>.Fail(userResult.Error);

        var user = userResult.Value;

        var roleResult = await _roleService.AssignRolesAsync(user.Id, command.Roles, cancellationToken);

        if (!roleResult.IsSuccess)
            return Result<UserResponse>.Fail(roleResult.Error);

        var emailResult = await SendConfirmationEmailAsync(user, cancellationToken);

        if (!emailResult.IsSuccess)
            return Result<UserResponse>.Fail(emailResult.Error);

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
