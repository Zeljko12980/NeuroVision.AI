namespace IdentityService.Application.Queries.User;

public sealed record GetUsersQuery(GetAllUsersRequest Request)
    : IQuery<Result<PaginatedResult<UserResponse>>>;

public sealed class GetUsersQueryHandler
    : IQueryHandler<GetUsersQuery, Result<PaginatedResult<UserResponse>>>
{
    private readonly IUserService _userService;

    public GetUsersQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<PaginatedResult<UserResponse>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        return await _userService.GetUsersAsync(
            request.Request.PageIndex,
            request.Request.PageSize,
            request.Request.Search,
            cancellationToken);
    }
}
