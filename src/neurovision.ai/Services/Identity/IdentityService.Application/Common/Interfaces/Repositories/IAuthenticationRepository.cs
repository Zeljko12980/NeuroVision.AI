namespace IdentityService.Application.Common.Interfaces.Repositories
{
    public interface IAuthenticationRepository
    {
        Task<bool> SignInAsync(string userName, string password);

        Task<(bool IsSuccess, string Code, string UserId, string Email)> LoginAsync(string email, string password);

        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<(bool IsSuccess, string Token)> ConfirmTwoFactorAsync(string email, string code);
    }
}
