using BuildingBlocks.Exceptions;
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
                throw new BadRequestException("Email confirmation failed.");

            var dto = repoResult.Adapt<ConfirmEmailResponse>();
            return Result<ConfirmEmailResponse>.Ok(dto);
        }

        public async Task<Result<AuthResponse>> LoginAsync(string email, string password)
        {
            var repoResult = await _authenticationRepository.LoginAsync(email, password);

            if (!repoResult.IsSuccess)
                throw new UnauthorizedException("Incorrect email or password.");

            await Publish2FACode(repoResult.Email, repoResult.Code);

            return Result<AuthResponse>.Ok(new AuthResponse
            {
                Email = repoResult.Email,
                Message = "Two-factor code sent to email."
            });
        }

        public async Task<Result<SignInResponse>> SignInAsync(string userName, string password)
        {
            var repoResult = await _authenticationRepository.SignInAsync(userName, password);

            if (!repoResult)
                throw new UnauthorizedException("Sign-in failed.");

            var dto = repoResult.Adapt<SignInResponse>();
            return Result<SignInResponse>.Ok(dto);
        }

        public async Task<Result<Confirm2FAResponse>> ConfirmTwoFactorAsync(string email, string code)
        {
            var result = await _authenticationRepository.ConfirmTwoFactorAsync(email, code);

            if (!result.IsSuccess)
                throw new UnauthorizedException("Invalid or expired 2FA code.");

            return Result<Confirm2FAResponse>.Ok(new Confirm2FAResponse
            {
                Token = result.Token,
                Message = "Login successful."
            });
        }

        public async Task<Result<Confirm2FAResponse>> ResendTwoFactorCodeAsync(string email)
        {
            var repoResult = await _authenticationRepository.ResendTwoFactorCodeAsync(email);

            if (!repoResult.IsSuccess)
                throw new BadRequestException("Failed to resend 2FA code.");

            await Publish2FACode(email, repoResult.Code);

            return Result<Confirm2FAResponse>.Ok(new Confirm2FAResponse
            {
                Message = "New two-factor code sent to email."
            });
        }

        private async Task Publish2FACode(string email, string code)
        {
            await _publishEndpoint.Publish(new TwoFactorCodeGeneratedEvent(email, code));
        }

        public async Task<Result<ForgotPasswordResponse>> ForgotPasswordAsync(string email)
        {
            var repoResult = await _authenticationRepository.GeneratePasswordResetTokenAsync(email);

            if (!repoResult.IsSuccess)
                throw new BadRequestException("User not found or email not confirmed.");

            return Result<ForgotPasswordResponse>.Ok(new ForgotPasswordResponse
            {
                EmailSent = true,
                Message = "Password reset email sent successfully."
            });
        }

        public async Task<Result<ResetPasswordResponse>> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var success = await _authenticationRepository.ResetPasswordAsync(email, token, newPassword);

            if (!success)
                throw new BadRequestException("Failed to reset password. Invalid token or email.");

            return Result<ResetPasswordResponse>.Ok(new ResetPasswordResponse
            {
                PasswordReset = true,
                Message = "Password reset successfully."
            });
        }
    }
}