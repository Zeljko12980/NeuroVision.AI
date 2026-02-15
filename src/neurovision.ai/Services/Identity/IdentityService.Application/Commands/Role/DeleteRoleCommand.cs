namespace IdentityService.Application.Commands.Role
{
    public class DeleteRoleCommand : ICommand<Result<OperationResponse>>
    {
        public string RoleId { get; init; } = string.Empty;
    }

    public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
    {
        public DeleteRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("RoleId is required.");
        }
    }


    public class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand, Result<OperationResponse>>
    {
        private readonly IRoleService _roleService;

        public DeleteRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result<OperationResponse>> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
        {
            return await _roleService.DeleteRoleAsync(command.RoleId);
        }
    }


}
