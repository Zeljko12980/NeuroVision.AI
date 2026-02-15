namespace IdentityService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ISender _sender;

        public UserController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserCommand command)
        {
            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            var result = await _sender.Send(new DeleteUserCommand { UserId = userId });
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateProfileCommand command)
        {
            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPost("check-password")]
        public async Task<IActionResult> CheckPassword(CheckPasswordCommand command)
        {
            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(string userId)
        {
            var result = await _sender.Send(new GetUserByIdQuery { UserId = userId });
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpGet("by-username/{userName}")]
        public async Task<IActionResult> GetByUserName(string userName)
        {
            var result = await _sender.Send(new GetUserByUserNameQuery { UserName = userName });
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var result = await _sender.Send(new GetUserByEmailQuery { Email = email });
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpGet("id/{userName}")]
        public async Task<IActionResult> GetUserId(string userName)
        {
            var result = await _sender.Send(new GetUserIdQuery { UserName = userName });
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _sender.Send(new GetAllUsersQuery());
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }
    }

}
