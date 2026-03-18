using IdentityService.Application.Common.Requests;

namespace IdentityService.Application.Commands.Authentication
{
    public record LoginCommand(LoginRequest LoginRequest) : ICommand<Result<AuthResponse>>;


    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.LoginRequest.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.LoginRequest.Password)
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
            return await _authService.LoginAsync(command.LoginRequest.Email, command.LoginRequest.Password);
        }
    }

}
