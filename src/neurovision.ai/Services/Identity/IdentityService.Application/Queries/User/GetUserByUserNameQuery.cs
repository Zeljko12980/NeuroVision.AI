namespace IdentityService.Application.Queries.User
{
    public class GetUserByUserNameQuery : IQuery<Result<UserResponse>>
    {
        public string UserName { get; init; } = string.Empty;
    }

    public class GetUserByUserNameQueryHandler : IQueryHandler<GetUserByUserNameQuery, Result<UserResponse>>
    {
        private readonly IUserService _userService;

        public GetUserByUserNameQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<UserResponse>> Handle(GetUserByUserNameQuery query, CancellationToken cancellationToken)
        {
            return await _userService.GetByUserNameAsync(query.UserName);
        }
    }


}
