namespace IdentityService.Application.Handlers;

public class UserDeletedEventHandler : IConsumer<DeleteUserEvent>
{
    private readonly IUserService _userService;
    private readonly ILogger<UserDeletedEventHandler> _logger;

    public UserDeletedEventHandler(
        IUserService userService,
        ILogger<UserDeletedEventHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DeleteUserEvent> context)
    {
        var userId = context.Message.UserId;

        var result = await _userService.DeleteUserAsync(userId, context.CancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("User {UserId} already deleted", userId);
                return;
            }

            _logger.LogError("Failed to delete user {UserId}: {Error}",
                userId, result.Error);

            throw new InvalidOperationException(result.Error);
        }

        _logger.LogInformation("User {UserId} successfully deleted", userId);
    }
}
