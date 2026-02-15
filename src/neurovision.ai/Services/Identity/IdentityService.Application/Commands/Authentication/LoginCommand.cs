namespace IdentityService.Application.Commands.Authentication
{
    public class LoginCommand : ICommand<Result<AuthResponse>>
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }


    public class LoginCommandHandler : ICommandHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IAuthenticationService _authService;

        public LoginCommandHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<Result<AuthResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            return await _authService.LoginAsync(command.Email, command.Password);
        }
    }

}
