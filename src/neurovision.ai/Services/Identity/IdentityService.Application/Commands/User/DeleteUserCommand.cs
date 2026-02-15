namespace IdentityService.Application.Commands.User
{
    public class DeleteUserCommand : ICommand<Result<OperationResponse>>
    {
        public string UserId { get; init; } = string.Empty;
    }

    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .Must(id => Guid.TryParse(id, out _))
                .WithMessage("UserId must be a valid GUID.");
        }
    }


    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, Result<OperationResponse>>
    {
        private readonly IUserService _userService;

        public DeleteUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<OperationResponse>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            return await _userService.DeleteAsync(command.UserId);
        }
    }


}
