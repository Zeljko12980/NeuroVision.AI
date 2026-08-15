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

    public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IAuthenticationService _authenticationService;

        public LoginCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result<AuthResponse>> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            return await _authenticationService.LoginAsync(
                request.LoginRequest.Email,
                request.LoginRequest.Password,
                cancellationToken);
        }
    }
}
