namespace IdentityService.Application.Commands.User
{
    public class UpdateProfileCommand : ICommand<Result<OperationResponse>>
    {
        public string UserId { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
    }

    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .Must(id => Guid.TryParse(id, out _))
                .WithMessage("UserId must be a valid GUID.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }


    public class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, Result<OperationResponse>>
    {
        private readonly IUserService _userService;

        public UpdateProfileCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<OperationResponse>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
        {
            return await _userService.UpdateProfileAsync(
                command.UserId,
                command.FirstName,
                command.LastName,
                command.Email
            );
        }
    }

}
