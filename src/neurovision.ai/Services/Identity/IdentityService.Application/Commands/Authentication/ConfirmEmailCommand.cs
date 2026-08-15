namespace IdentityService.Application.Commands.Authentication
{
    public class ConfirmEmailCommand : ICommand<Result<ConfirmEmailResponse>>
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }

    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Confirmation token is required.");
        }
    }

    public class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand, Result<ConfirmEmailResponse>>
    {
        private readonly IAuthenticationService _authService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IConfiguration _configuration;

        public ConfirmEmailCommandHandler(
            IAuthenticationService authService,
            IPublishEndpoint publishEndpoint,
            IConfiguration configuration)
        {
            _authService = authService;
            _publishEndpoint = publishEndpoint;
            _configuration = configuration;
        }

        public async Task<Result<ConfirmEmailResponse>> Handle(
            ConfirmEmailCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _authService.ConfirmEmailAsync(command.Email, command.Token);

            if (!result.IsSuccess)
                return Result<ConfirmEmailResponse>.Fail(result.Error);

            var tokenResult = await _authService.GenerateSetPasswordTokenAsync(command.Email);

            if (!tokenResult.IsSuccess)
                return Result<ConfirmEmailResponse>.Fail(tokenResult.Error);

            var frontendUrl = _configuration["AppSettings:FrontendUrl"];

            if (string.IsNullOrEmpty(frontendUrl))
                return Result<ConfirmEmailResponse>.Fail("Frontend URL is not configured.");

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tokenResult.Value));

            var link = $"{frontendUrl}/set-password?email={command.Email}&token={encodedToken}";

            await _publishEndpoint.Publish(new SetPasswordEvent(command.Email, link));

            return Result<ConfirmEmailResponse>.Ok(new ConfirmEmailResponse
            {
                IsConfirmed = true
            });
        }
    }
}
