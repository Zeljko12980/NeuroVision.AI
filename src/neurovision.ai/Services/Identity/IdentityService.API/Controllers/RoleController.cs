namespace IdentityService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "SuperAdminPolicy")]
    public class RoleController : ControllerBase
    {
        private readonly ISender _sender;

        public RoleController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            var result = await _sender.Send(new CreateRoleCommand(
                request.RoleName,
                request.Description));

            return result.ToActionResult(role =>
                CreatedAtAction(
                    nameof(GetById),
                    new { roleId = role.Id },
                    role));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _sender.Send(new DeleteRoleCommand(id));

            return result.ToActionResult();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request)
        {
            var result = await _sender.Send(
                new UpdateRoleCommand(id, request.RoleName, request.Description));

            return result.ToActionResult(role => Ok(role));
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllRolesRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetRolesQuery(request),
                cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{roleId:guid}")]
        public async Task<IActionResult> GetById( Guid roleId, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetRoleByIdQuery(roleId),
                cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserRoles(Guid userId,CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetUserRolesQuery(userId),
                cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRoles([FromBody] AssignRolesRequest request,CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new AssignRolesCommand(request.UserId, request.Roles),
                cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut("update-user-roles")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesRequest request,CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new UpdateUserRolesCommand(
             request.UserId,
             request.Roles),
             cancellationToken);

            return result.ToActionResult();
        }
    }
}