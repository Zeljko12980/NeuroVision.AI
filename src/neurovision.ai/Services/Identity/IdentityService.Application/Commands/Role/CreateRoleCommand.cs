namespace IdentityService.Application.Commands.Role
{
    public record CreateRoleCommand(string RoleName, string? Description) : ICommand<Result<RoleResponse>>;


    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
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

    public class CreateRoleCommandHandler
     : ICommandHandler<CreateRoleCommand, Result<RoleResponse>>
    {
        private readonly IRoleService _roleService;

        public CreateRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result<RoleResponse>> Handle(
            CreateRoleCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _roleService.CreateRoleAsync(
                command.RoleName,
                command.Description,
                cancellationToken);

            if (!result.IsSuccess)
                return Result<RoleResponse>.Fail(result.Error, result.StatusCode);

            return result.Map(role => role.Adapt<RoleResponse>());
        }
    }
}


