namespace IdentityService.Application.Commands.Authentication
{
    public class ConfirmEmailCommand : ICommand<Result<ConfirmEmailResponse>>
    {
        public string UserId { get; init; } = string.Empty;
        public string Token { get; init; } = string.Empty;
    }

    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .Must(id => Guid.TryParse(id, out _))
                .WithMessage("UserId must be a valid GUID.");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Confirmation token is required.");
        }
    }


    public class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand, Result<ConfirmEmailResponse>>
    {
        private readonly IAuthenticationService _authService;

        public ConfirmEmailCommandHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<Result<ConfirmEmailResponse>> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            return await _authService.ConfirmEmailAsync(command.UserId, command.Token);
        }
    }


}
