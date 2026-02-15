namespace IdentityService.Application.Queries.User
{
    public class GetAllUsersQuery : IQuery<Result<List<UserResponse>>>
    {
    }

    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, Result<List<UserResponse>>>
    {
        private readonly IUserService _userService;

        public GetAllUsersQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<List<UserResponse>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            return await _userService.GetAllAsync(cancellationToken);
        }
    }

}
