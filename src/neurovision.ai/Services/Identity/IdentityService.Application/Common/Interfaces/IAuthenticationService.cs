namespace IdentityService.Application.Common.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Result<SignInResponse>> SignInAsync(string userName, string password);
        Task<Result<AuthResponse>> LoginAsync(string email, string password);
        Task<Result<ConfirmEmailResponse>> ConfirmEmailAsync(string userId, string token);
        Task<Result<Confirm2FAResponse>> ConfirmTwoFactorAsync(string email, string code);
        Task<Result<Confirm2FAResponse>> ResendTwoFactorCodeAsync(string email);
        Task<Result<ForgotPasswordResponse>> ForgotPasswordAsync(string email);
        Task<Result<ResetPasswordResponse>> ResetPasswordAsync(string email, string token, string newPassword);
    }

}
