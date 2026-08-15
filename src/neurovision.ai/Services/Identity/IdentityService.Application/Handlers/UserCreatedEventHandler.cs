using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace IdentityService.Application.Handlers
{
    public class UserCreatedEventHandler : IConsumer<CreateUserEvent>
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IConfiguration _configuration;
        private readonly IFileStorageService _fileStorageService;

        public UserCreatedEventHandler(
            IUserService userService,
            IRoleService roleService,
            IPublishEndpoint publishEndpoint,
            IConfiguration configuration,
            IFileStorageService fileStorageService
            )
        {
            _userService = userService;
            _roleService = roleService;
            _publishEndpoint = publishEndpoint;
            _configuration = configuration;
            _fileStorageService = fileStorageService;
        }

        public async Task Consume(ConsumeContext<CreateUserEvent> context)
        {
            var message = context.Message;

            var user = await CreateUserAsync(message);

            await AssignRoleAsync(message, context.CancellationToken);

            await SendConfirmationEmailAsync(user);

        }



        private async Task<UserResponse> CreateUserAsync(CreateUserEvent message)
        {

            var user = await _userService.CreateAsync(
               message.UserId,
               message.Username,
               message.Email);

            return new UserResponse
            {
                Id = user.Value.Id,
                Email = user.Value.Email,
                UserName = user.Value.UserName
            };
        }
        private async Task AssignRoleAsync(CreateUserEvent message, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(message.RoleName))
                return;

            await _roleService.AssignRolesAsync(
                message.UserId,
                new List<string> { message.RoleName }, cancellationToken);
        }

        private async Task SendConfirmationEmailAsync(UserResponse user)
        {
            var tokenResult = await _userService.GenerateEmailConfirmationTokenAsync(user.Id);

            if (!tokenResult.IsSuccess)
                throw new Exception("Frontend URL is not configured");

            var frontendUrl = _configuration["AppSettings:FrontendUrl"];

            if (string.IsNullOrEmpty(frontendUrl))
                throw new Exception("Frontend URL is not configured");

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tokenResult.Value));

            var link =
                $"{frontendUrl}/confirm-email?email={user.Email}&token={encodedToken}";



            await _publishEndpoint.Publish(new ConfirmEmailEvent(
                user.Id,
                user.Email!,
                link));
        }
    }
}