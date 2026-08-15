namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.SuperAdmin)]
public class UserController : ControllerBase
{
    private readonly ISender _sender;

    public UserController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var command = new CreateUserCommand
        {
            Id = request.Id,
            UserName = request.UserName,
            Email = request.Email,
            Roles = request.Roles,
        };

        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
    {
        var result = await _sender.Send(new ConfirmEmailCommand
        {
            Email = request.Email,
            Token = request.Token
        });

        return result.ToActionResult();
    }
}
