using Microsoft.Extensions.Logging;
using System.Net;

namespace IdentityService.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityService _identityService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IJwtTokenGenerator _jwt;
        private readonly IUserService _userService;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            IIdentityService identityService,
            IPublishEndpoint publishEndpoint,
            IJwtTokenGenerator jwt,
            IUserService userService,
            ILogger<AuthenticationService> logger)
        {
            _identityService = identityService;
            _publishEndpoint = publishEndpoint;
            _jwt = jwt;
            _userService = userService;
            _logger = logger;
        }

        public async Task<Result<SignInResponse>> SignInAsync(string userName, string password)
        {
            var success = await _identityService.SignInAsync(userName, password);

            if (!success)
                return Result<SignInResponse>.Fail("Invalid username or password.");

            return Result<SignInResponse>.Ok(new SignInResponse
            {
                IsSignedIn = true,
                Message = "Sign-in successful."
            });
        }

        public async Task<Result<AuthResponse>> LoginAsync(string email, string password,CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Login started. Email={Email}",
                email);

            var success = await _identityService.SignInAsync(email, password);

            if (!success)
            {
                _logger.LogWarning(
                    "Login failed. Invalid credentials. Email={Email}",
                    email);

                return Result<AuthResponse>.Fail(
                    "Invalid credentials.",
                    HttpStatusCode.Unauthorized);
            }

            _logger.LogDebug(
                "Credentials validated. Generating 2FA code. Email={Email}",
                email);

            var code = await _identityService.GenerateTwoFactorCodeAsync(email);

            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogError(
                    "Failed to generate 2FA code. Email={Email}",
                    email);

                return Result<AuthResponse>.Fail(
                    "Unable to generate two-factor authentication code.",
                    HttpStatusCode.InternalServerError);
            }

            await PublishTwoFactorCode(email, code);

            _logger.LogInformation(
                "2FA code generated and published. Email={Email}",
                email);

            return Result<AuthResponse>.Ok(
                new AuthResponse
                {
                    Email = email,
                    Message = "Two-factor authentication code sent successfully."
                });
        }

        public async Task<Result<ConfirmEmailResponse>> ConfirmEmailAsync(string email, string token)
        {
            var success = await _identityService.ConfirmEmailAsync(email, token);

            if (!success)
                return Result<ConfirmEmailResponse>.Fail("Email confirmation failed.");

            return Result<ConfirmEmailResponse>.Ok(new ConfirmEmailResponse
            {
                IsConfirmed = true,
                Message = "Email confirmed successfully."
            });
        }

        public async Task<Result<Confirm2FAResponse>> ConfirmTwoFactorAsync(string email, string code)
        {
            var verified = await _identityService.VerifyTwoFactorAsync(email, code);

            if (!verified)
                return Result<Confirm2FAResponse>.Fail("Invalid or expired 2FA code.");

            var rolesResult = await _identityService.GetUserRolesAsync(email);

            if (rolesResult is null)
                return Result<Confirm2FAResponse>.Fail("Roles not found.");

            var roles = rolesResult ?? new List<string>();

            var userResult = await _userService.GetByEmailAsync(email);

            if (userResult == null)
                return Result<Confirm2FAResponse>.Fail("User not found.");

            var token = _jwt.GenerateToken(userResult.Value.Id, email,userResult.Value.UserName, roles.ToList());

            return Result<Confirm2FAResponse>.Ok(new Confirm2FAResponse
            {
                Token = token,
                Message = "Login successful."
            });
        }

        public async Task<Result<Confirm2FAResponse>> ResendTwoFactorCodeAsync(string email)
        {
            var code = await _identityService.GenerateTwoFactorCodeAsync(email);

            if (code is null)
                return Result<Confirm2FAResponse>.Fail("Unable to generate 2FA code.");

            await PublishTwoFactorCode(email, code);

            return Result<Confirm2FAResponse>.Ok(new Confirm2FAResponse
            {
                Message = "New two-factor code sent."
            });
        }

        public async Task<Result<ForgotPasswordResponse>> ForgotPasswordAsync(string email)
        {
            var token = await _identityService.GeneratePasswordResetTokenAsync(email);

            if (token is null)
                return Result<ForgotPasswordResponse>.Fail("User not found.");

            return Result<ForgotPasswordResponse>.Ok(new ForgotPasswordResponse
            {
                EmailSent = true,
                Message = "Password reset token generated."
            });
        }

        public async Task<Result<ResetPasswordResponse>> ResetPasswordAsync(
            string email,
            string token,
            string newPassword)
        {
            var success = await _identityService.ResetPasswordAsync(email, token, newPassword);

            if (!success)
                return Result<ResetPasswordResponse>.Fail("Invalid token or email.");

            return Result<ResetPasswordResponse>.Ok(new ResetPasswordResponse
            {
                PasswordReset = true,
                Message = "Password reset successful."
            });
        }

        private async Task PublishTwoFactorCode(string email, string code)
        {
            _logger.LogDebug(
                "Publishing TwoFactorCodeGeneratedEvent. Email={Email}",
                email);

            var userName = await _identityService.GetUserNameByEmailAsync(email) ?? email;

            await _publishEndpoint.Publish(
                new TwoFactorCodeGeneratedEvent(email, code, userName));

            _logger.LogDebug(
                "TwoFactorCodeGeneratedEvent published successfully. Email={Email}",
                email);
        }

        public async Task<Result<string>> GenerateSetPasswordTokenAsync(string email)
        {
            var token = await _identityService.GeneratePasswordResetTokenAsync(email);

            if (string.IsNullOrEmpty(token))
                return Result<string>.Fail("User not found or token generation failed.");

            return Result<string>.Ok(token);
        }

        public async Task<Result> SetPasswordWithTokenAsync(
          string email,
          string token,
          string password)
        {
            var success = await _identityService.ResetPasswordAsync(email, token, password);

            if (!success)
                return Result.Fail("Invalid token or user not found.");


            return Result.Ok();
        }
    }
}