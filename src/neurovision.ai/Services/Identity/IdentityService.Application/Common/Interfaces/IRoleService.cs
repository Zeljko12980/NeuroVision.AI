namespace IdentityService.Application.Common.Interfaces;

public interface IRoleService
{
    Task<Result<Role>> CreateRoleAsync(string roleName, string? description, CancellationToken cancellationToken);

    Task<Result<Role>> UpdateRoleAsync(Guid roleId, string roleName, string? description, CancellationToken cancellationToken);

    Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken);

    Task<Result<PaginatedResult<RoleResponse>>> GetRolesAsync(int pageIndex, int pageSize, string? roleName, CancellationToken cancellationToken);

    Task<Result<Role>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken);

    Task<Result<List<string>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken);

    Task<Result> AssignRolesAsync(Guid userId, IList<string> roles, CancellationToken cancellationToken);

    Task<Result<List<Role>>> UpdateUserRolesAsync(Guid userId, IList<string> roles, CancellationToken cancellationToken);
}
