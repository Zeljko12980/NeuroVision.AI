namespace IdentityService.Application.Queries.Role;

public sealed record GetRolesQuery(GetAllRolesRequest Request)
    : IQuery<Result<PaginatedResult<RoleResponse>>>;

public sealed class GetRolesQueryHandler
    : IQueryHandler<GetRolesQuery, Result<PaginatedResult<RoleResponse>>>
{
    private readonly IRoleService _roleService;

    public GetRolesQueryHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<Result<PaginatedResult<RoleResponse>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        return await _roleService.GetRolesAsync(
            request.Request.PageIndex,
            request.Request.PageSize,
            request.Request.RoleName,
            cancellationToken);
    }
}
