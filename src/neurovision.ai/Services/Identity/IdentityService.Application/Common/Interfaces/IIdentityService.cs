namespace IdentityService.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<bool> SignInAsync(string email, string password);

        Task<string?> GenerateTwoFactorCodeAsync(string email);

        Task<bool> VerifyTwoFactorAsync(string email, string code);

        Task<IList<string>?> GetUserRolesAsync(string email);

        Task<bool> ConfirmEmailAsync(string email, string token);

        Task<string?> GeneratePasswordResetTokenAsync(string email);

        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

        Task<string?> GetUserNameByEmailAsync(string email);
    }
}
