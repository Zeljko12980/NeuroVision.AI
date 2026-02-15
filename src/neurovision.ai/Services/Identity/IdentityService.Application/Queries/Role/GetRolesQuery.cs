namespace IdentityService.Application.Queries.Role
{
    public class GetRolesQuery : IQuery<Result<List<RoleResponse>>>
    {
    }

    public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, Result<List<RoleResponse>>>
    {
        private readonly IRoleService _roleService;

        public GetRolesQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result<List<RoleResponse>>> Handle(GetRolesQuery query, CancellationToken cancellationToken)
        {
            return await _roleService.GetRolesAsync();
        }
    }


}
