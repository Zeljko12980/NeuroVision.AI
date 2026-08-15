namespace IdentityService.Application.Commands.Authentication
{
    public class SignInCommand : ICommand<Result<SignInResponse>>
    {
        public string UserName { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }

    public class SignInCommandValidator : AbstractValidator<SignInCommand>
    {
        public SignInCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }


    public class SignInCommandHandler : ICommandHandler<SignInCommand, Result<SignInResponse>>
    {
        private readonly IAuthenticationService _authService;

        public SignInCommandHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<Result<SignInResponse>> Handle(SignInCommand command, CancellationToken cancellationToken)
        {
            return await _authService.SignInAsync(command.UserName, command.Password);
        }
    }


}
