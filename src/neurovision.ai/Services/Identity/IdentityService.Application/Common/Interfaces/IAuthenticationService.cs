namespace IdentityService.Application.Common.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Result<SignInResponse>> SignInAsync(string userName, string password);
        Task<Result<AuthResponse>> LoginAsync(string email, string password,CancellationToken cancellationToken);
        Task<Result<ConfirmEmailResponse>> ConfirmEmailAsync(string email, string token);
        Task<Result<Confirm2FAResponse>> ConfirmTwoFactorAsync(string email, string code);
        Task<Result<Confirm2FAResponse>> ResendTwoFactorCodeAsync(string email);
        Task<Result<ForgotPasswordResponse>> ForgotPasswordAsync(string email);
        Task<Result<ResetPasswordResponse>> ResetPasswordAsync(string email, string token, string newPassword);
        Task<Result<string>> GenerateSetPasswordTokenAsync(string email);

        Task<Result> SetPasswordWithTokenAsync(string email, string token, string password);
    }

}
