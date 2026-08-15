namespace IdentityService.Application.Commands.Role
{
    public record UpdateRoleCommand(Guid RoleId, string RoleName, string? Description) : ICommand<Result<RoleResponse>>;


    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("RoleId is required.");

            RuleFor(x => x.RoleName)
             .NotEmpty().WithMessage("Role name is required.")
             .MaximumLength(50)
             .Matches(@"^[a-zA-Z0-9_\-\.]+$")
             .WithMessage("Role name can only contain letters, numbers, underscore, dash and dot.")
             .Must(name => name == name.Trim())
             .WithMessage("Role name must not contain leading or trailing spaces.");

            RuleFor(x => x.Description)
               .MaximumLength(250)
                   .WithMessage("Description must not exceed 250 characters.");
        }
    }

    public class UpdateRoleCommandHandler
       : ICommandHandler<UpdateRoleCommand, Result<RoleResponse>>
    {
        private readonly IRoleService _roleService;

        public UpdateRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result<RoleResponse>> Handle(
         UpdateRoleCommand command,
         CancellationToken cancellationToken)
        {
            var result = await _roleService.UpdateRoleAsync(
                command.RoleId,
                command.RoleName,
                command.Description,
                cancellationToken);

            return result.Map(role => role.Adapt<RoleResponse>());
        }
    }
}

