using IdentityService.Application.Common.Requests;

namespace IdentityService.Application.Commands.Authentication
{
    public record Confirm2FACommand(Confirm2FARequest Confirm2FARequest) : ICommand<Result<Confirm2FAResponse>>;

    public class Confirm2FACommandValidator : AbstractValidator<Confirm2FACommand>
    {
        public Confirm2FACommandValidator()
        {
            RuleFor(x => x.Confirm2FARequest.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Confirm2FARequest.Code)
                .NotEmpty().WithMessage("2FA code is required.")
                .Length(6).WithMessage("2FA code must be 6 characters long.");
        }
    }

    public class Confirm2FACommandHandler : ICommandHandler<Confirm2FACommand, Result<Confirm2FAResponse>>
    {
        private readonly IAuthenticationService _authService;
        public Confirm2FACommandHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }
        public async Task<Result<Confirm2FAResponse>> Handle(Confirm2FACommand command, CancellationToken cancellationToken)
        {
            return await _authService.ConfirmTwoFactorAsync(command.Confirm2FARequest.Email, command.Confirm2FARequest.Code);
        }
    }
}
