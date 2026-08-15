using BuildingBlocks.Results;

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
        public async Task<IActionResult> ConfirmTwoFactor([FromBody] Confirm2FARequest request)
        {
            var result = await _sender.Send(new Confirm2FACommand(request));
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPost("resend-2fa")]
        public async Task<IActionResult> ResendTwoFactor([FromBody] Resend2FARequest request)
        {
            var result = await _sender.Send(new Resend2FACommand(request));
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
        {
            var result = await _sender.Send(new SetPasswordCommand(
                request.Email,
                request.Token,
                request.Password));

            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result.Error);
        }
    }
}