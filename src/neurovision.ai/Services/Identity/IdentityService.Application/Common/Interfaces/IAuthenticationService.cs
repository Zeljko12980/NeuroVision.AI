namespace IdentityService.Application.Common.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Result<SignInResponse>> SignInAsync(string userName, string password);
        Task<Result<AuthResponse>> LoginAsync(string email, string password);
        Task<Result<ConfirmEmailResponse>> ConfirmEmailAsync(string userId, string token);
        Task<Result<Confirm2FAResponse>> ConfirmTwoFactorAsync(string email, string code);
    }

}
