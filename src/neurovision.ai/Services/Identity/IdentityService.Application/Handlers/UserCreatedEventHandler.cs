namespace IdentityService.Application.Handlers;

public class UserCreatedEventHandler : IConsumer<CreateUserEvent>
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IFrontendLinkService _frontendLinkService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(
        IUserService userService,
        IRoleService roleService,
        IFrontendLinkService frontendLinkService,
        IPublishEndpoint publishEndpoint,
        ILogger<UserCreatedEventHandler> logger)
    {
        _userService = userService;
        _roleService = roleService;
        _frontendLinkService = frontendLinkService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CreateUserEvent> context)
    {
        var message = context.Message;
        var cancellationToken = context.CancellationToken;

        _logger.LogInformation(
            "CreateUserEvent received. UserId={UserId}, Email={Email}, RoleName={RoleName}",
            message.UserId,
            message.Email,
            message.RoleName);

        var userResult = await _userService.CreateAsync(
            message.UserId,
            message.Username,
            message.Email,
            cancellationToken);

        if (!userResult.IsSuccess)
        {
            _logger.LogError(
                "CreateUserEvent failed while creating user. UserId={UserId}, Error={Error}",
                message.UserId,
                userResult.Error);
            throw new InvalidOperationException(userResult.Error);
        }

        var user = userResult.Value;

        if (!string.IsNullOrEmpty(message.RoleName))
        {
            var roleResult = await _roleService.AssignRolesAsync(
                message.UserId,
                new List<string> { message.RoleName },
                cancellationToken);

            if (!roleResult.IsSuccess)
            {
                _logger.LogError(
                    "CreateUserEvent failed while assigning role. UserId={UserId}, RoleName={RoleName}, Error={Error}",
                    message.UserId,
                    message.RoleName,
                    roleResult.Error);
                throw new InvalidOperationException(roleResult.Error);
            }
        }

        var tokenResult = await _userService.GenerateEmailConfirmationTokenAsync(user.Id, cancellationToken);

        if (!tokenResult.IsSuccess)
        {
            _logger.LogError(
                "CreateUserEvent failed while generating confirmation token. UserId={UserId}, Error={Error}",
                user.Id,
                tokenResult.Error);
            throw new InvalidOperationException(tokenResult.Error);
        }

        var linkResult = _frontendLinkService.BuildConfirmEmailLink(user.Email, tokenResult.Value);

        if (!linkResult.IsSuccess)
        {
            _logger.LogError(
                "CreateUserEvent failed while building confirmation link. UserId={UserId}, Error={Error}",
                user.Id,
                linkResult.Error);
            throw new InvalidOperationException(linkResult.Error);
        }

        await _publishEndpoint.Publish(new ConfirmEmailEvent(
            user.Id,
            user.Email,
            linkResult.Value),
            cancellationToken);

        _logger.LogInformation("CreateUserEvent processed successfully. UserId={UserId}, Email={Email}", user.Id, user.Email);
    }
}
