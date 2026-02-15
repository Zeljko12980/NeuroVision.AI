namespace IdentityService.Application.Queries.Role
{
    public class GetUserRolesQuery : IQuery<Result<UserRolesResponse>>
    {
        public string UserId { get; init; } = string.Empty;
    }

    public class GetUserRolesQueryValidator : AbstractValidator<GetUserRolesQuery>
    {
        public GetUserRolesQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }

    public class GetUserRolesQueryHandler : IQueryHandler<GetUserRolesQuery, Result<UserRolesResponse>>
    {
        private readonly IRoleService _roleService;

        public GetUserRolesQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result<UserRolesResponse>> Handle(GetUserRolesQuery query, CancellationToken cancellationToken)
        {
            return await _roleService.GetUserRolesAsync(query.UserId);
        }
    }


}
