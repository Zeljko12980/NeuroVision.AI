namespace IdentityService.Application.Handlers;

public class UserCreatedEventHandler : IConsumer<CreateUserEvent>
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IFrontendLinkService _frontendLinkService;
    private readonly IPublishEndpoint _publishEndpoint;

    public UserCreatedEventHandler(
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

    public async Task Consume(ConsumeContext<CreateUserEvent> context)
    {
        var message = context.Message;
        var cancellationToken = context.CancellationToken;

        var userResult = await _userService.CreateAsync(
            message.UserId,
            message.Username,
            message.Email,
            cancellationToken);

        if (!userResult.IsSuccess)
            throw new InvalidOperationException(userResult.Error);

        var user = userResult.Value;

        if (!string.IsNullOrEmpty(message.RoleName))
        {
            var roleResult = await _roleService.AssignRolesAsync(
                message.UserId,
                new List<string> { message.RoleName },
                cancellationToken);

            if (!roleResult.IsSuccess)
                throw new InvalidOperationException(roleResult.Error);
        }

        var tokenResult = await _userService.GenerateEmailConfirmationTokenAsync(user.Id, cancellationToken);

        if (!tokenResult.IsSuccess)
            throw new InvalidOperationException(tokenResult.Error);

        var linkResult = _frontendLinkService.BuildConfirmEmailLink(user.Email, tokenResult.Value);

        if (!linkResult.IsSuccess)
            throw new InvalidOperationException(linkResult.Error);

        await _publishEndpoint.Publish(new ConfirmEmailEvent(
            user.Id,
            user.Email,
            linkResult.Value),
            cancellationToken);
    }
}
