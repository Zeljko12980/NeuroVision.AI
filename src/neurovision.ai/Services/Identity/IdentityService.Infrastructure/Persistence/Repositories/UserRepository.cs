namespace IdentityService.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly IdentityContext _context;

        public UserRepository(
            UserManager<User> userManager,
            IdentityContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateAsync(
            string username,
            string password,
            string email,
            string firstName,
            string lastName)
        {
            var user = new User(username, email, firstName, lastName);


            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
                return (true, user.Id, Enumerable.Empty<string>());

            return (false, string.Empty, result.Errors.Select(e => e.Description));
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<User?> GetByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<string?> GetUserIdAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user?.Id;
        }

        public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateProfileAsync(string userId, string firstName, string lastName, string email)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            user.UpdateName(firstName, lastName);
            user.Email = email;
            user.UserName = email;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}

