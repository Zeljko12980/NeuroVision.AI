namespace IdentityService.Application.Queries.Authentication;

public record GetCurrentUserQuery : IQuery<Result<UserResponse>>;

public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, Result<UserResponse>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ILogger<GetCurrentUserQueryHandler> _logger;

    public GetCurrentUserQueryHandler(
        ICurrentUser currentUser,
        IUserService userService,
        IRoleService roleService,
        ILogger<GetCurrentUserQueryHandler> logger)
    {
        _currentUser = currentUser;
        _userService = userService;
        _roleService = roleService;
        _logger = logger;
    }

    public async Task<Result<UserResponse>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is not Guid userId)
        {
            _logger.LogWarning("Get current user failed. User is not authenticated.");
            return Result<UserResponse>.Fail("Unauthorized.", HttpStatusCode.Unauthorized);
        }

        var userResult = await _userService.GetByIdAsync(userId, cancellationToken);
        if (!userResult.IsSuccess)
            return Result<UserResponse>.Fail(userResult.Error, userResult.StatusCode);

        var response = userResult.Value.ToResponse();

        var rolesResult = await _roleService.GetUserRolesAsync(userId, cancellationToken);
        if (rolesResult.IsSuccess)
            response.Roles = rolesResult.Value;

        return Result<UserResponse>.Ok(response);
    }
}
