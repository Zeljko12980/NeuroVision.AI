namespace IdentityService.Application.Common.Interfaces
{
    public interface IRoleService
    {
        Task<Result<OperationResponse>> CreateRoleAsync(string roleName);
        Task<Result<OperationResponse>> DeleteRoleAsync(string roleId);
        Task<Result<OperationResponse>> UpdateRoleAsync(string roleId, string roleName);
        Task<Result<List<RoleResponse>>> GetRolesAsync();
        Task<Result<RoleResponse>> GetByIdAsync(string roleId);
        Task<Result<UserRolesResponse>> GetUserRolesAsync(string userId);
        Task<Result<OperationResponse>> AssignRolesAsync(string userName, IList<string> roles);
        Task<Result<OperationResponse>> UpdateUserRolesAsync(string userName, IList<string> roles);
    }

}
