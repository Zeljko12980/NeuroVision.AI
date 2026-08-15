namespace IdentityService.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<AspIdentityUser> _userManager;
        private readonly SignInManager<AspIdentityUser> _signInManager;

        public IdentityService(
            UserManager<AspIdentityUser> userManager,
            SignInManager<AspIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<bool> SignInAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return false;

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            return result.Succeeded;
        }

        public async Task<string?> GenerateTwoFactorCodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return null;

            return await _userManager.GenerateTwoFactorTokenAsync(
                user,
                TokenOptions.DefaultEmailProvider);
        }

        public async Task<bool> VerifyTwoFactorAsync(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return false;

            return await _userManager.VerifyTwoFactorTokenAsync(
                user,
                TokenOptions.DefaultEmailProvider,
                code);
        }

        public async Task<IList<string>?> GetUserRolesAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return null;

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return false;

            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                return false;

            user.TwoFactorEnabled = true;
            await _userManager.UpdateAsync(user);

            return true;
        }

        public async Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return null;

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<bool> ResetPasswordAsync(
            string email,
            string token,
            string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return false;

            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);

            return result.Succeeded;
        }

        public async Task<string?> GetUserNameByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user?.UserName;
        }
    }
}
