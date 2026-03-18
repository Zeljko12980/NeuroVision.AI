using IdentityService.Application.Common.Requests;

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
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var result = await _sender.Send(new LoginCommand(loginRequest));
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPost("confirm-2fa")]
        public async Task<IActionResult> ConfirmTwoFactor([FromBody] Confirm2FARequest request)
        {
            var result = await _sender.Send(new Confirm2FACommand(request));
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);


        }
    }
}
