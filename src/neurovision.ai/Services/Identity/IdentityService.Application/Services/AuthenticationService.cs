namespace IdentityService.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;

        public AuthenticationService(IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
        }
        public Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<(bool IsSuccess, string Token, string UserId, string UserName)> LoginAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SignInAsync(string userName, string password)
        {
            throw new NotImplementedException();
        }
    }
}
