namespace IdentityService.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<AspIdentityRole> _roleManager;
        private readonly UserManager<AspIdentityUser> _userManager;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            RoleManager<AspIdentityRole> roleManager,
            UserManager<AspIdentityUser> userManager,
            ILogger<RoleService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result<RoleDto>> CreateRoleAsync(string roleName, string? description,CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateRoleAsync started. RoleName={RoleName}", roleName);

            if (await _roleManager.RoleExistsAsync(roleName))
            {
                _logger.LogWarning("CreateRole failed - role already exists. RoleName={RoleName}", roleName);

                return Result<RoleDto>.Fail("Role already exists", HttpStatusCode.Conflict);
            }

            var role = new AspIdentityRole(Guid.NewGuid(), roleName, description);

            _logger.LogDebug("Creating role entity. RoleName={RoleName}", roleName);

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(x => x.Description));

                _logger.LogError("Role creation failed. RoleName={RoleName}, Errors={Errors}", roleName, error);

                return Result<RoleDto>.Fail("Creation failed", HttpStatusCode.BadRequest);
            }

            _logger.LogInformation("Role created successfully. RoleId={RoleId}, RoleName={RoleName}", role.Id, roleName);

            return Result<RoleDto>.Created(role.Adapt<RoleDto>());
        }
        public async Task<Result<RoleDto>> UpdateRoleAsync(Guid roleId,string roleName,string? description,CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "UpdateRoleAsync started. RoleId={RoleId}, RoleName={RoleName}",
                roleId,
                roleName);

            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null)
            {
                _logger.LogWarning("Update failed - role not found. RoleId={RoleId}", roleId);

                return Result<RoleDto>.Fail("Role not found.", HttpStatusCode.NotFound);
            }

            if (!role.Name!.Equals(roleName, StringComparison.OrdinalIgnoreCase)
                && await _roleManager.RoleExistsAsync(roleName))
            {
                _logger.LogWarning(
                    "Update failed - role name already exists. RoleId={RoleId}, RoleName={RoleName}",
                    roleId,
                    roleName);

                return Result<RoleDto>.Fail("Role already exists.", HttpStatusCode.Conflict);
            }

            _logger.LogDebug(
                "Updating role. RoleId={RoleId}, OldName={OldName}, NewName={NewName}",
                roleId,
                role.Name,
                roleName);

            role.Name = roleName;
            role.NormalizedName = _roleManager.NormalizeKey(roleName);
            role.SetDescription(description);

            var identityResult = await _roleManager.UpdateAsync(role);

            if (!identityResult.Succeeded)
            {
                var error = string.Join(", ", identityResult.Errors.Select(x => x.Description));

                _logger.LogError(
                    "Role update failed. RoleId={RoleId}, Errors={Errors}",
                    roleId,
                    error);

                return Result<RoleDto>.Fail(error, HttpStatusCode.BadRequest);
            }

            _logger.LogInformation("Role updated successfully. RoleId={RoleId}", roleId);

            return Result<RoleDto>.Ok(role.Adapt<RoleDto>());
        }

        public async Task<Result> DeleteRoleAsync(Guid roleId,CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteRoleAsync started. RoleId={RoleId}", roleId);

            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null)
            {
                _logger.LogWarning("Delete failed - role not found. RoleId={RoleId}", roleId);

                return Result.Fail("Role not found.", HttpStatusCode.NotFound);
            }

            _logger.LogDebug("Deleting role. RoleId={RoleId}, RoleName={RoleName}", roleId, role.Name);

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(x => x.Description));

                _logger.LogError(
                    "Role deletion failed. RoleId={RoleId}, Errors={Errors}",
                    roleId,
                    error);

                return Result.Fail(error, HttpStatusCode.BadRequest);
            }

            _logger.LogInformation("Role deleted successfully. RoleId={RoleId}", roleId);

            return Result.NoContent();
        }

        public async Task<Result<PaginatedResult<RoleDto>>> GetRolesAsync(int pageIndex,int pageSize,string? roleName,CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Loading roles. PageIndex={PageIndex}, PageSize={PageSize}, RoleName={RoleName}",
                pageIndex,
                pageSize,
                roleName);

            IQueryable<AspIdentityRole> query = _roleManager.Roles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                query = query.Where(x => x.Name!.Contains(roleName));
            }

            var totalCount = await query.LongCountAsync(cancellationToken);

            var roles = await query
                .OrderBy(x => x.Name)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var items = new List<RoleDto>(roles.Count);

            foreach (var role in roles)
            {
                var users = await _userManager.GetUsersInRoleAsync(role.Name!);

                items.Add(new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name!,
                    Description = role.Description,
                    UserCount = users.Count,
                    Status = users.Count > 0 ? "Active" : "Inactive"
                });
            }

            _logger.LogInformation(
                "Loaded {Count} roles from total {TotalCount}",
                items.Count,
                totalCount);

            return Result<PaginatedResult<RoleDto>>.Ok(
                new PaginatedResult<RoleDto>(
                    pageIndex,
                    pageSize,
                    totalCount,
                    items));
        }
        public async Task<Result<RoleDto>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Loading role. RoleId={RoleId}",
                roleId);

            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role is null)
            {
                _logger.LogWarning(
                    "Role not found. RoleId={RoleId}",
                    roleId);

                return Result<RoleDto>.Fail(
                    "Role not found.",
                    HttpStatusCode.NotFound);
            }

            _logger.LogInformation(
                "Role loaded successfully. RoleId={RoleId}",
                roleId);

            return Result<RoleDto>.Ok(role.Adapt<RoleDto>());
        }

        public async Task<Result<List<string>>> GetUserRolesAsync(Guid userId,CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Loading roles for user. UserId={UserId}",
                userId);

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                _logger.LogWarning(
                    "User not found. UserId={UserId}",
                    userId);

                return Result<List<string>>.Fail(
                    "User not found.",
                    HttpStatusCode.NotFound);
            }

            var roles = await _userManager.GetRolesAsync(user);

            _logger.LogInformation(
                "Loaded {Count} roles for user {UserId}",
                roles.Count,
                userId);

            return Result<List<string>>.Ok(roles.ToList());
        }

        public async Task<Result> AssignRolesAsync(Guid userId,IList<string> roles,CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Assigning roles. UserId={UserId}",
                userId);

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                _logger.LogWarning(
                    "User not found. UserId={UserId}",
                    userId);

                return Result.Fail(
                    "User not found.",
                    HttpStatusCode.NotFound);
            }

            var result = await _userManager.AddToRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));

                _logger.LogWarning(
                    "Failed to assign roles to user {UserId}. Errors: {Errors}",
                    userId,
                    error);

                return Result.Fail(
                    error,
                    HttpStatusCode.BadRequest);
            }

            _logger.LogInformation(
                "Roles assigned successfully. UserId={UserId}",
                userId);

            return Result.Ok();
        }

        public async Task<Result<List<RoleDto>>> UpdateUserRolesAsync(Guid userId,IList<string> roles,CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Updating roles for user {UserId}",
                userId);

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                _logger.LogWarning(
                    "User not found. UserId={UserId}",
                    userId);

                return Result<List<RoleDto>>.Fail(
                    "User not found.",
                    HttpStatusCode.NotFound);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!removeResult.Succeeded)
                {
                    return Result<List<RoleDto>>.Fail(
                        string.Join(", ", removeResult.Errors.Select(x => x.Description)),
                        HttpStatusCode.BadRequest);
                }
            }

            if (roles.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, roles);

                if (!addResult.Succeeded)
                {
                    return Result<List<RoleDto>>.Fail(
                        string.Join(", ", addResult.Errors.Select(x => x.Description)),
                        HttpStatusCode.BadRequest);
                }
            }

            var updatedRoles = await _roleManager.Roles
                .AsNoTracking()
                .Where(r => roles.Contains(r.Name!))
                .ToListAsync(cancellationToken);

            var response = updatedRoles.Select(role => new RoleDto
            {
                Id = role.Id,
                Name = role.Name!,
                Description = role.Description
            }).ToList();

            _logger.LogInformation(
                "User roles updated successfully. UserId={UserId}",
                userId);

            return Result<List<RoleDto>>.Ok(response);
        }
    }
}