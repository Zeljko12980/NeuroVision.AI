namespace IdentityService.Infrastructure.Services;

public class AspNetIdentityService : IIdentityService
{
    private readonly UserManager<AspIdentityUser> _userManager;
    private readonly SignInManager<AspIdentityUser> _signInManager;
    private readonly ILogger<AspNetIdentityService> _logger;

    public AspNetIdentityService(
        UserManager<AspIdentityUser> userManager,
        SignInManager<AspIdentityUser> signInManager,
        ILogger<AspNetIdentityService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<bool> SignInAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            _logger.LogWarning("Sign-in failed. User not found. Email={Email}", email);
            return false;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Sign-in failed. Invalid password. Email={Email}", email);
            return false;
        }

        _logger.LogDebug("Sign-in credentials validated. Email={Email}", email);
        return true;
    }

    public async Task<string?> GenerateTwoFactorCodeAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            _logger.LogWarning("2FA code generation failed. User not found. Email={Email}", email);
            return null;
        }

        var code = await _userManager.GenerateTwoFactorTokenAsync(
            user,
            TokenOptions.DefaultEmailProvider);

        _logger.LogInformation("2FA code generated. Email={Email}", email);
        return code;
    }

    public async Task<bool> VerifyTwoFactorAsync(string email, string code, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            _logger.LogWarning("2FA verification failed. User not found. Email={Email}", email);
            return false;
        }

        var verified = await _userManager.VerifyTwoFactorTokenAsync(
            user,
            TokenOptions.DefaultEmailProvider,
            code);

        if (!verified)
        {
            _logger.LogWarning("2FA verification failed. Invalid or expired code. Email={Email}", email);
            return false;
        }

        _logger.LogInformation("2FA verified successfully. Email={Email}", email);
        return true;
    }

    public async Task<IList<string>?> GetUserRolesAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            _logger.LogWarning("Get user roles failed. User not found. Email={Email}", email);
            return null;
        }

        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            _logger.LogWarning("Email confirmation failed. User not found. Email={Email}", email);
            return false;
        }

        var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
        {
            var error = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Email confirmation failed. Email={Email}, Errors={Errors}", email, error);
            return false;
        }

        user.TwoFactorEnabled = true;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("Email confirmed successfully. Email={Email}", email);
        return true;
    }

    public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            _logger.LogWarning("Password reset token generation failed. User not found. Email={Email}", email);
            return null;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        _logger.LogInformation("Password reset token generated. Email={Email}", email);
        return token;
    }

    public async Task<bool> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            _logger.LogWarning("Password reset failed. User not found. Email={Email}", email);
            return false;
        }

        var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);

        if (!result.Succeeded)
        {
            var error = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Password reset failed. Email={Email}, Errors={Errors}", email, error);
            return false;
        }

        _logger.LogInformation("Password reset successfully. Email={Email}", email);
        return true;
    }

    public async Task<string?> GetUserNameByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user?.UserName;
    }
}
