namespace IdentityService.Application.Commands.Role
{
    public sealed record UpdateUserRolesCommand(Guid UserId,IList<string> Roles)
    : ICommand<Result<List<RoleResponse>>>;


    public class UpdateUserRolesCommandValidator : AbstractValidator<UpdateUserRolesCommand>
    {
        public UpdateUserRolesCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.Roles)
                .NotEmpty().WithMessage("At least one role must be provided.")
                .Must(r => r.All(x => !string.IsNullOrWhiteSpace(x)))
                .WithMessage("Roles cannot contain empty values.");
        }
    }

    public sealed class UpdateUserRolesCommandHandler
    : ICommandHandler<UpdateUserRolesCommand, Result<List<RoleResponse>>>
    {
        private readonly IRoleService _roleService;

        public UpdateUserRolesCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result<List<RoleResponse>>> Handle(
            UpdateUserRolesCommand request,
            CancellationToken cancellationToken)
        {
            return (await _roleService.UpdateUserRolesAsync(
                    request.UserId,
                    request.Roles,
                    cancellationToken))
                .Map(x => x.Adapt<List<RoleResponse>>());
        }
    }

}
