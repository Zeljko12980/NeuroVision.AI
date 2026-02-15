namespace IdentityService.Application.Queries.User
{
    public class GetUserByEmailQuery : IQuery<Result<UserResponse>>
    {
        public string Email { get; init; } = string.Empty;
    }

    public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, Result<UserResponse>>
    {
        private readonly IUserService _userService;

        public GetUserByEmailQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<UserResponse>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
        {
            return await _userService.GetByEmailAsync(query.Email);
        }
    }


}
