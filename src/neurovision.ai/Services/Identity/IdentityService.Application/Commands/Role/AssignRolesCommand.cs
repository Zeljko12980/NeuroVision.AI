namespace IdentityService.Application.Commands.Role
{
    public sealed record AssignRolesCommand(Guid UserId, IList<string> Roles) : ICommand<Result>;

    public class AssignRolesCommandValidator : AbstractValidator<AssignRolesCommand>
    {
        public AssignRolesCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.Roles)
                .NotEmpty().WithMessage("At least one role must be assigned.")
                .Must(r => r.All(role => !string.IsNullOrWhiteSpace(role)))
                .WithMessage("Roles cannot contain empty values.");
        }
    }


    public sealed class AssignRolesCommandHandler: ICommandHandler<AssignRolesCommand, Result>
    {
        private readonly IRoleService _roleService;

        public AssignRolesCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result> Handle(
            AssignRolesCommand request,
            CancellationToken cancellationToken)
        {
            return await _roleService.AssignRolesAsync(
                request.UserId,
                request.Roles,
                cancellationToken);
        }
    }


}
