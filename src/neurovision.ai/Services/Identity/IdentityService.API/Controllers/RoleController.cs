namespace IdentityService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly ISender _sender;

        public RoleController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateRoleCommand command)
        {
            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> Delete(string roleId)
        {
            var result = await _sender.Send(new DeleteRoleCommand { RoleId = roleId });
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateRoleCommand command)
        {
            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _sender.Send(new GetRolesQuery());
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetById(string roleId)
        {
            var result = await _sender.Send(new GetRoleByIdQuery { RoleId = roleId });
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var result = await _sender.Send(new GetUserRolesQuery { UserId = userId });
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRoles(AssignRolesCommand command)
        {
            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPut("update-user-roles")]
        public async Task<IActionResult> UpdateUserRoles(UpdateUserRolesCommand command)
        {
            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
    }

}
