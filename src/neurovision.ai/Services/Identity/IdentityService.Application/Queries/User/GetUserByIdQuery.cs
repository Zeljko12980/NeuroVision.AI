namespace IdentityService.Application.Queries.User
{
    public class GetUserByIdQuery : IQuery<Result<UserResponse>>
    {
        public string UserId { get; init; } = string.Empty;
    }

    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Result<UserResponse>>
    {
        private readonly IUserService _userService;

        public GetUserByIdQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            return await _userService.GetByIdAsync(query.UserId);
        }
    }

}
