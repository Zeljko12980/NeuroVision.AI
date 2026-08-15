namespace IdentityService.Application.Queries.Role
{
    public sealed record GetUserRolesQuery(Guid UserId)
    : IQuery<Result<UserRolesResponse>>;

    public sealed class GetUserRolesQueryHandler
        : IQueryHandler<GetUserRolesQuery, Result<UserRolesResponse>>
    {
        private readonly IRoleService _roleService;

        public GetUserRolesQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result<UserRolesResponse>> Handle(
            GetUserRolesQuery request,
            CancellationToken cancellationToken)
        {
            return (await _roleService.GetUserRolesAsync(
                    request.UserId,
                    cancellationToken))
                .Map(roles => new UserRolesResponse
                {
                    Roles = roles
                });
        }
    }
}
