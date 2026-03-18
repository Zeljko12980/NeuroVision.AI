using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace IdentityService.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public AuthenticationService(IAuthenticationRepository authenticationRepository, IPublishEndpoint publishEndpoint)
        {
            _authenticationRepository = authenticationRepository;
            _publishEndpoint = publishEndpoint;
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
            try
            {
                var repoResult = await _authenticationRepository.LoginAsync(email, password);

                if (!repoResult.IsSuccess)
                    return Result<AuthResponse>.Fail("Invalid credentials.");

                await _publishEndpoint.Publish(new TwoFactorCodeGeneratedEvent(
                    repoResult.Email,
                    repoResult.Code
                ));

                return Result<AuthResponse>.Ok(new AuthResponse
                {
                    Email = repoResult.Email,
                    Message = "Two-factor code sent to email."
                });
            }
            catch (Exception ex)
            {
                return Result<AuthResponse>.Fail(ex.Message);
            }
        }

        public async Task<Result<SignInResponse>> SignInAsync(string userName, string password)
        {
            var repoResult = await _authenticationRepository.SignInAsync(userName, password);

            if (!repoResult)
                return Result<SignInResponse>.Fail("Sign-in failed.");

            var dto = repoResult.Adapt<SignInResponse>();
            return Result<SignInResponse>.Ok(dto);
        }

        public async Task<Result<Confirm2FAResponse>> ConfirmTwoFactorAsync(string email, string code)
        {
            var result = await _authenticationRepository.ConfirmTwoFactorAsync(email, code);

            if (!result.IsSuccess)
                return Result<Confirm2FAResponse>.Fail("Invalid or expired 2FA code.");

            return Result<Confirm2FAResponse>.Ok(new Confirm2FAResponse
            {
                Token = result.Token,
                Message = "Login successful."
            });
        }

    }
}
