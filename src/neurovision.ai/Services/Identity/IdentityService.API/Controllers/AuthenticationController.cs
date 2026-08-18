namespace IdentityService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthenticationController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new LoginCommand(request),
                cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost("confirm-2fa")]
        public async Task<IActionResult> ConfirmTwoFactor([FromBody] Confirm2FARequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new Confirm2FACommand(request), cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost("resend-2fa")]
        public async Task<IActionResult> ResendTwoFactor([FromBody] Resend2FARequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new Resend2FACommand(request), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new SetPasswordCommand(
                request.Email,
                request.Token,
                request.Password), cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new ForgotPasswordCommand(request.Email), cancellationToken);
            return result.ToActionResult();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new ChangePasswordCommand(request.CurrentPassword, request.NewPassword),
                cancellationToken);
            return result.ToActionResult();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetCurrentUserQuery(), cancellationToken);
            return result.ToActionResult();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new UpdateProfileCommand(request.UserName, request.PhoneNumber),
                cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new ConfirmEmailCommand
            {
                Email = request.Email,
                Token = request.Token
            }, cancellationToken);

            return result.ToActionResult();
        }
    }
}