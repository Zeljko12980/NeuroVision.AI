namespace IdentityService.Application.Commands.User
{
    public class CheckPasswordCommand : ICommand<Result<OperationResponse>>
    {
        public IdentityService.Domain.Entities.User User { get; init; } = default!;
        public string Password { get; init; } = string.Empty;
    }

    public class CheckPasswordCommandValidator : AbstractValidator<CheckPasswordCommand>
    {
        public CheckPasswordCommandValidator()
        {
            RuleFor(x => x.User)
                .NotNull().WithMessage("User entity must be provided.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }


    public class CheckPasswordCommandHandler : ICommandHandler<CheckPasswordCommand, Result<OperationResponse>>
    {
        private readonly IUserService _userService;

        public CheckPasswordCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<OperationResponse>> Handle(CheckPasswordCommand command, CancellationToken cancellationToken)
        {
            return await _userService.CheckPasswordAsync(command.User, command.Password);
        }
    }


}
