namespace IdentityService.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<Result<OperationResponse>> CreateRoleAsync(string roleName)
        {
            var result = await _roleRepository.CreateRoleAsync(roleName);
            if (!result) return Result<OperationResponse>.Fail("Role creation failed.");

            return Result<OperationResponse>.Ok(new OperationResponse { Success = true });
        }

        public async Task<Result<OperationResponse>> DeleteRoleAsync(string roleId)
        {
            var result = await _roleRepository.DeleteRoleAsync(roleId);
            if (!result) return Result<OperationResponse>.Fail("Role deletion failed.");

            return Result<OperationResponse>.Ok(new OperationResponse { Success = true });
        }

        public async Task<Result<OperationResponse>> UpdateRoleAsync(string roleId, string roleName)
        {
            var result = await _roleRepository.UpdateRoleAsync(roleId, roleName);
            if (!result) return Result<OperationResponse>.Fail("Role update failed.");

            return Result<OperationResponse>.Ok(new OperationResponse { Success = true });
        }

        public async Task<Result<List<RoleResponse>>> GetRolesAsync()
        {
            var roles = await _roleRepository.GetRolesAsync();
            var dto = roles.Adapt<List<RoleResponse>>();
            return Result<List<RoleResponse>>.Ok(dto);
        }

        public async Task<Result<RoleResponse>> GetByIdAsync(string roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null) return Result<RoleResponse>.Fail("Role not found.");

            var dto = role.Value.Adapt<RoleResponse>();
            return Result<RoleResponse>.Ok(dto);
        }

        public async Task<Result<UserRolesResponse>> GetUserRolesAsync(string userId)
        {
            var roles = await _roleRepository.GetUserRolesAsync(userId);
            if (roles == null || roles.Count == 0)
                return Result<UserRolesResponse>.Fail("No roles found for user.");

            var dto = new UserRolesResponse { Roles = roles };
            return Result<UserRolesResponse>.Ok(dto);
        }

        public async Task<Result<OperationResponse>> AssignRolesAsync(string userName, IList<string> roles)
        {
            var result = await _roleRepository.AssignRolesAsync(userName, roles);
            if (!result) return Result<OperationResponse>.Fail("Assigning roles failed.");

            return Result<OperationResponse>.Ok(new OperationResponse { Success = true });
        }

        public async Task<Result<OperationResponse>> UpdateUserRolesAsync(string userName, IList<string> roles)
        {
            var result = await _roleRepository.UpdateUserRolesAsync(userName, roles);
            if (!result) return Result<OperationResponse>.Fail("Updating user roles failed.");

            return Result<OperationResponse>.Ok(new OperationResponse { Success = true });
        }

    }
}
