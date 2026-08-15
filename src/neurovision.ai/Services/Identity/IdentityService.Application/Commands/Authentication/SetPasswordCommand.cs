namespace IdentityService.Application.Commands.Authentication
{
    public record SetPasswordCommand(
        string Email,
        string Token,
        string Password
    ) : ICommand<Result>;

    public class SetPasswordCommandValidator : AbstractValidator<SetPasswordCommand>
    {
        public SetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Token)
                .NotEmpty();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);
        }
    }

    public class SetPasswordCommandHandler
       : ICommandHandler<SetPasswordCommand, Result>
    {
        private readonly IAuthenticationService _authService;

        public SetPasswordCommandHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(SetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _authService.SetPasswordWithTokenAsync(
                command.Email,
                command.Token,
                command.Password);

            if (!result.IsSuccess)
                return Result.Fail(result.Error);

            return Result.Ok();
        }
    }
}
