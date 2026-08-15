namespace IdentityService.Application.Common.Interfaces
{
    public interface IRoleService
    {
        Task<Result<RoleDto>> CreateRoleAsync(string roleName, string? description, CancellationToken cancellationToken);

        Task<Result<RoleDto>> UpdateRoleAsync(Guid roleId, string roleName, string? description, CancellationToken cancellationToken);

        Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken);

        Task<Result<PaginatedResult<RoleDto>>> GetRolesAsync(int pageIndex, int pageSize, string? roleName, CancellationToken cancellationToken);

        Task<Result<RoleDto>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken);

        Task<Result<List<string>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken);

        Task<Result> AssignRolesAsync(Guid userId, IList<string> roles, CancellationToken cancellationToken);

        Task<Result<List<RoleDto>>> UpdateUserRolesAsync(Guid userId, IList<string> roles, CancellationToken cancellationToken);
    }
}
