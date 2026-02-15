namespace IdentityService.Application.Commands.Role
{
    public class AssignRolesCommand : ICommand<Result<OperationResponse>>
    {
        public string UserName { get; init; } = string.Empty;
        public IList<string> Roles { get; init; } = new List<string>();
    }

    public class AssignRolesCommandValidator : AbstractValidator<AssignRolesCommand>
    {
        public AssignRolesCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.");

            RuleFor(x => x.Roles)
                .NotEmpty().WithMessage("At least one role must be assigned.");
        }
    }


    public class AssignRolesCommandHandler : ICommandHandler<AssignRolesCommand, Result<OperationResponse>>
    {
        private readonly IRoleService _roleService;

        public AssignRolesCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result<OperationResponse>> Handle(AssignRolesCommand command, CancellationToken cancellationToken)
        {
            return await _roleService.AssignRolesAsync(command.UserName, command.Roles);
        }
    }


}
