namespace IdentityService.Application.Commands.User
{
    public class CreateUserCommand : ICommand<Result<UserResponse>>
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID is required.");

            RuleFor(x => x.UserName)
                .NotEmpty().MinimumLength(3);

            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress();

            RuleFor(x => x.Roles)
                .NotNull();
        }
    }

    public class CreateUserCommandHandler
        : ICommandHandler<CreateUserCommand, Result<UserResponse>>
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IConfiguration _configuration;

        public CreateUserCommandHandler(
            IUserService userService,
            IRoleService roleService,
            IPublishEndpoint publishEndpoint,
            IConfiguration configuration)
        {
            _userService = userService;
            _roleService = roleService;
            _publishEndpoint = publishEndpoint;
            _configuration = configuration;
        }

        public async Task<Result<UserResponse>> Handle(
            CreateUserCommand command,
            CancellationToken cancellationToken)
        {
            var userResult = await _userService.CreateAsync(
                command.Id,
                command.UserName,
                command.Email);

            if (!userResult.IsSuccess)
                return Result<UserResponse>.Fail(userResult.Error);

            var user = userResult.Value;

            var roleResult = await _roleService.AssignRolesAsync(user.Id, command.Roles, cancellationToken);

            if (!roleResult.IsSuccess)
                return Result<UserResponse>.Fail(roleResult.Error);

            var emailResult = await SendConfirmationEmailAsync(user.Id, user.Email);

            if (!emailResult.IsSuccess)
                return Result<UserResponse>.Fail(emailResult.Error);

            return Result<UserResponse>.Ok(new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            });
        }

        private async Task<Result> SendConfirmationEmailAsync(Guid userId, string email)
        {
            var tokenResult = await _userService.GenerateEmailConfirmationTokenAsync(userId);

            if (!tokenResult.IsSuccess)
                return Result.Fail(tokenResult.Error);

            var frontendUrl = _configuration["AppSettings:FrontendUrl"];

            if (string.IsNullOrEmpty(frontendUrl))
                return Result.Fail("Frontend URL is not configured.");

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tokenResult.Value));

            var link = $"{frontendUrl}/confirm-email?email={email}&token={encodedToken}";

            await _publishEndpoint.Publish(new ConfirmEmailEvent(
                userId,
                email,
                link));

            return Result.Ok();
        }
    }
}
