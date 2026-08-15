namespace IdentityService.Application.Commands.Role
{
    public record DeleteRoleCommand(Guid RoleId) : ICommand<Result>;

    public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
    {
        public DeleteRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("RoleId is required.");
        }
    }


    public class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand, Result>
    {
        private readonly IRoleService _roleService;

        public DeleteRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
        {
            return await _roleService.DeleteRoleAsync(command.RoleId, cancellationToken);
        }
    }


}
