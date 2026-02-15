namespace IdentityService.Application.Commands.Role
{
    public class UpdateRoleCommand : ICommand<Result<OperationResponse>>
    {
        public string RoleId { get; init; } = string.Empty;
        public string RoleName { get; init; } = string.Empty;
    }

    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("RoleId is required.");

            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.");
        }
    }

    public class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand, Result<OperationResponse>>
    {
        private readonly IRoleService _roleService;

        public UpdateRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result<OperationResponse>> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
        {
            return await _roleService.UpdateRoleAsync(command.RoleId, command.RoleName);
        }
    }


}
