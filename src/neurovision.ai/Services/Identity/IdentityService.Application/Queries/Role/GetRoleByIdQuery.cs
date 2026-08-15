namespace IdentityService.Application.Queries.Role;

public sealed record GetRoleByIdQuery(Guid RoleId)
    : IQuery<Result<RoleResponse>>;

public sealed class GetRoleByIdQueryHandler
    : IQueryHandler<GetRoleByIdQuery, Result<RoleResponse>>
{
    private readonly IRoleService _roleService;

    public GetRoleByIdQueryHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<Result<RoleResponse>> Handle(
        GetRoleByIdQuery request,
        CancellationToken cancellationToken)
    {
        return (await _roleService.GetByIdAsync(
                request.RoleId,
                cancellationToken))
            .Map(role => role.ToResponse());
    }
}
