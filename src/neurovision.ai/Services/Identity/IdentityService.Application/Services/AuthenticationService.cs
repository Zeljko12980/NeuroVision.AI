namespace IdentityService.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;

        public AuthenticationService(IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
        }

        public async Task<Result<ConfirmEmailResponse>> ConfirmEmailAsync(string userId, string token)
        {
            var repoResult = await _authenticationRepository.ConfirmEmailAsync(userId, token);

            if (!repoResult)
                return Result<ConfirmEmailResponse>.Fail("Email confirmation failed.");

            var dto = repoResult.Adapt<ConfirmEmailResponse>();
            return Result<ConfirmEmailResponse>.Ok(dto);
        }

        public async Task<Result<AuthResponse>> LoginAsync(string email, string password)
        {
            var repoResult = await _authenticationRepository.LoginAsync(email, password);

            if (!repoResult.IsSuccess)
                return Result<AuthResponse>.Fail("Invalid credentials.");

            var dto = repoResult.Adapt<AuthResponse>();
            return Result<AuthResponse>.Ok(dto);
        }

        public async Task<Result<SignInResponse>> SignInAsync(string userName, string password)
        {
            var repoResult = await _authenticationRepository.SignInAsync(userName, password);

            if (!repoResult)
                return Result<SignInResponse>.Fail("Sign-in failed.");

            var dto = repoResult.Adapt<SignInResponse>();
            return Result<SignInResponse>.Ok(dto);
        }

    }
}
