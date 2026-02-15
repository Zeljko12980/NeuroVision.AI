namespace IdentityService.Application.Queries.User
{
    public class GetUserIdQuery : IQuery<Result<string>>
    {
        public string UserName { get; init; } = string.Empty;
    }

    public class GetUserIdQueryHandler : IQueryHandler<GetUserIdQuery, Result<string>>
    {
        private readonly IUserService _userService;

        public GetUserIdQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<string>> Handle(GetUserIdQuery query, CancellationToken cancellationToken)
        {
            return await _userService.GetUserIdAsync(query.UserName);
        }
    }


}
