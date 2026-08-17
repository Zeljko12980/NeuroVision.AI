namespace IdentityService.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<SignInStatus> SignInAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<string?> GenerateTwoFactorCodeAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> VerifyTwoFactorAsync(string email, string code, CancellationToken cancellationToken = default);

    Task<IList<string>?> GetUserRolesAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken = default);

    Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);

    Task<string?> GetUserNameByEmailAsync(string email, CancellationToken cancellationToken = default);
}
