using IdentityService.Application.Common.Requests;

namespace IdentityService.Application.Commands.Authentication
{
    public record Resend2FACommand(Resend2FARequest Resend2FARequest)
         : ICommand<Result<Confirm2FAResponse>>;

    public class Resend2FACommandValidator : AbstractValidator<Resend2FACommand>
    {
        public Resend2FACommandValidator()
        {
            RuleFor(x => x.Resend2FARequest.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }

    public class Resend2FACommandHandler
      : ICommandHandler<Resend2FACommand, Result<Confirm2FAResponse>>
    {
        private readonly IAuthenticationService _authService;

        public Resend2FACommandHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<Result<Confirm2FAResponse>> Handle(
            Resend2FACommand command,
            CancellationToken cancellationToken)
        {
            return await _authService
                .ResendTwoFactorCodeAsync(command.Resend2FARequest.Email);
        }
    }
}
