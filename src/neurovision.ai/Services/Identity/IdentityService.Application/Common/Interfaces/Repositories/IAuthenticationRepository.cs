namespace IdentityService.Application.Common.Interfaces.Repositories
{
    public interface IAuthenticationRepository
    {
        Task<bool> SignInAsync(string userName, string password);

        Task<(bool IsSuccess, string Token, string UserId, string UserName)>
            LoginAsync(string email, string password);

        Task<bool> ConfirmEmailAsync(string userId, string token);
    }
}
