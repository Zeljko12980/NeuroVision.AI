namespace IdentityService.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IdentityContext _context;

        public RoleRepository(
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            IdentityContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> CreateRoleAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                return false;

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            return result.Succeeded;
        }

        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return false;

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }

        public async Task<(string Id, string Name)?> GetByIdAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return null;

            return (role.Id, role.Name!);
        }

        public async Task<List<(string Id, string Name)>> GetRolesAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .Select(r => new ValueTuple<string, string>(r.Id, r.Name!))
                .ToListAsync();
        }

        public async Task<bool> UpdateRoleAsync(string roleId, string roleName)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return false;

            role.Name = roleName;
            role.NormalizedName = roleName.ToUpper();

            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new List<string>();

            return (await _userManager.GetRolesAsync(user)).ToList();
        }

        public async Task<bool> AssignRolesAsync(string userName, IList<string> roles)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return false;

            var result = await _userManager.AddToRolesAsync(user, roles);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserRolesAsync(string userName, IList<string> roles)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return false;

            var currentRoles = await _userManager.GetRolesAsync(user);

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return false;

            var addResult = await _userManager.AddToRolesAsync(user, roles);
            return addResult.Succeeded;
        }
    }
}
