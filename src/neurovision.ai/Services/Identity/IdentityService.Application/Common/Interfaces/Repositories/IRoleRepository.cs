namespace IdentityService.Application.Common.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(string roleId);
        Task<bool> UpdateRoleAsync(string roleId, string roleName);

        Task<List<(string Id, string Name)>> GetRolesAsync();
        Task<(string Id, string Name)?> GetByIdAsync(string roleId);

        Task<List<string>> GetUserRolesAsync(string userId);

        Task<bool> AssignRolesAsync(string userName, IList<string> roles);
        Task<bool> UpdateUserRolesAsync(string userName, IList<string> roles);
    }
}
