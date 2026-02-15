namespace IdentityService.Application.Commands.Role
{
    public class UpdateUserRolesCommand : ICommand<Result<OperationResponse>>
    {
        public string UserName { get; init; } = string.Empty;
        public IList<string> Roles { get; init; } = new List<string>();
    }

    public class UpdateUserRolesCommandValidator : AbstractValidator<UpdateUserRolesCommand>
    {
        public UpdateUserRolesCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.");

            RuleFor(x => x.Roles)
                .NotEmpty().WithMessage("At least one role must be provided.");
        }
    }


    public class UpdateUserRolesCommandHandler : ICommandHandler<UpdateUserRolesCommand, Result<OperationResponse>>
    {
        private readonly IRoleService _roleService;

        public UpdateUserRolesCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result<OperationResponse>> Handle(UpdateUserRolesCommand command, CancellationToken cancellationToken)
        {
            return await _roleService.UpdateUserRolesAsync(command.UserName, command.Roles);
        }
    }


}
