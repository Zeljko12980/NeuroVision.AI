namespace IdentityService.Infrastructure.Persistence.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthenticationRepository(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> SignInAsync(string userName, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(
                userName,
                password,
                isPersistent: false,
                lockoutOnFailure: false);

            return result.Succeeded;
        }

        public async Task<(bool IsSuccess, string Token, string UserId, string UserName)>
            LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return (false, string.Empty, string.Empty, string.Empty);

            var passwordValid = await _userManager.CheckPasswordAsync(user, password);

            if (!passwordValid)
                return (false, string.Empty, string.Empty, string.Empty);

            if (!user.EmailConfirmed)
                return (false, string.Empty, string.Empty, string.Empty);

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            return (true, token, user.Id, user.UserName!);
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded;
        }
    }
}
