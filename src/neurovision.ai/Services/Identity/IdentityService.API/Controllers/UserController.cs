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
    public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand
        {
            Id = request.Id,
            UserName = request.UserName,
            Email = request.Email,
            Roles = request.Roles,
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllUsersRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUsersQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/unlock")]
    public async Task<IActionResult> Unlock(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UnlockUserCommand(id), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/lock")]
    public async Task<IActionResult> Lock(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new LockUserCommand(id), cancellationToken);
        return result.ToActionResult();
    }

}
