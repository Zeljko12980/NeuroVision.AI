namespace IdentityService.Application.Commands.Role
{
    public class CreateRoleCommand : ICommand<Result<OperationResponse>>
    {
        public string RoleName { get; init; } = string.Empty;
    }

    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.");
        }
    }


    public class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, Result<OperationResponse>>
    {
        private readonly IRoleService _roleService;

        public CreateRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result<OperationResponse>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
        {
            return await _roleService.CreateRoleAsync(command.RoleName);
        }
    }

}
