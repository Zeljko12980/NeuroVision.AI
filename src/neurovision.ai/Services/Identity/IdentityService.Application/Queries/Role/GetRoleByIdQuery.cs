namespace IdentityService.Application.Queries.Role
{
    public class GetRoleByIdQuery : IQuery<Result<RoleResponse>>
    {
        public string RoleId { get; init; } = string.Empty;
    }


    public class GetRoleByIdQueryHandler : IQueryHandler<GetRoleByIdQuery, Result<RoleResponse>>
    {
        private readonly IRoleService _roleService;

        public GetRoleByIdQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result<RoleResponse>> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken)
        {
            return await _roleService.GetByIdAsync(query.RoleId);
        }
    }


}
