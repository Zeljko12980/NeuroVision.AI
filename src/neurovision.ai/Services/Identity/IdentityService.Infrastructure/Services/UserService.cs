namespace IdentityService.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<AspIdentityUser> _userManager;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<AspIdentityUser> userManager,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<User>> CreateAsync(Guid id, string username, string email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user. UserId={UserId}, Email={Email}", id, email);

        var user = User.Create(id, username, email);
        var identityUser = AspIdentityUser.FromDomain(user);

        var result = await _userManager.CreateAsync(identityUser);

        if (!result.Succeeded)
        {
            var error = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("User creation failed. UserId={UserId}, Email={Email}, Errors={Errors}", id, email, error);

            return Result<User>.Fail(error);
        }

        _logger.LogInformation("User created successfully. UserId={UserId}, Email={Email}", identityUser.Id, email);

        return Result<User>.Ok(identityUser.ToDomain());
    }

    public async Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            _logger.LogWarning("User not found. Email={Email}", email);
            return Result<User>.Fail("User not found");
        }

        return Result<User>.Ok(user.ToDomain());
    }

    public async Task<Result<User>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning("User not found. UserId={UserId}", userId);
            return Result<User>.Fail("User not found", HttpStatusCode.NotFound);
        }

        return Result<User>.Ok(user.ToDomain());
    }

    public async Task<Result<User>> UpdateProfileAsync(
        Guid userId,
        string userName,
        string? phoneNumber,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating profile. UserId={UserId}", userId);

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning("Update profile failed. User not found. UserId={UserId}", userId);
            return Result<User>.Fail("User not found", HttpStatusCode.NotFound);
        }

        var trimmedName = userName.Trim();
        if (!string.Equals(user.UserName, trimmedName, StringComparison.Ordinal))
        {
            var userNameResult = await _userManager.SetUserNameAsync(user, trimmedName);
            if (!userNameResult.Succeeded)
            {
                var error = string.Join(", ", userNameResult.Errors.Select(e => e.Description));
                _logger.LogWarning(
                    "Update profile failed. UserName update failed. UserId={UserId}, Errors={Errors}",
                    userId,
                    error);
                return Result<User>.Fail(error);
            }
        }

        var normalizedPhone = InternationalPhoneNumber.Normalize(phoneNumber);
        if (!string.Equals(user.PhoneNumber, normalizedPhone, StringComparison.Ordinal))
        {
            var phoneResult = await _userManager.SetPhoneNumberAsync(user, normalizedPhone);
            if (!phoneResult.Succeeded)
            {
                var error = string.Join(", ", phoneResult.Errors.Select(e => e.Description));
                _logger.LogWarning(
                    "Update profile failed. PhoneNumber update failed. UserId={UserId}, Errors={Errors}",
                    userId,
                    error);
                return Result<User>.Fail(error);
            }
        }

        user.MarkUpdated();
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var error = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            _logger.LogWarning("Update profile failed. UserId={UserId}, Errors={Errors}", userId, error);
            return Result<User>.Fail(error);
        }

        _logger.LogInformation("Profile updated. UserId={UserId}", userId);
        return Result<User>.Ok(user.ToDomain());
    }

    public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning("Cannot generate email confirmation token. User not found. UserId={UserId}", userId);
            return Result<string>.Fail("User not found");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        _logger.LogInformation("Email confirmation token generated. UserId={UserId}", userId);

        return Result<string>.Ok(token);
    }

    public async Task<Result<PaginatedResult<UserResponse>>> GetUsersAsync(
        int pageIndex,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Loading users. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            pageSize,
            search);

        IQueryable<AspIdentityUser> query = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(user =>
                (user.UserName != null && user.UserName.Contains(term)) ||
                (user.Email != null && user.Email.Contains(term)));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        var users = await query
            .OrderBy(user => user.UserName)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = new List<UserResponse>(users.Count);

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var isLockedOut = await _userManager.IsLockedOutAsync(user)
                || (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow);

            items.Add(new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToList(),
                IsLockedOut = isLockedOut,
                LockoutEnd = user.LockoutEnd
            });
        }

        return Result<PaginatedResult<UserResponse>>.Ok(
            new PaginatedResult<UserResponse>(pageIndex, pageSize, totalCount, items));
    }

    public async Task<Result> UnlockAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Unlocking user. UserId={UserId}", userId);

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            _logger.LogWarning("Unlock failed. User not found. UserId={UserId}", userId);
            return Result.Fail("User not found", HttpStatusCode.NotFound);
        }

        var isLockedOut = await _userManager.IsLockedOutAsync(user);
        if (!isLockedOut)
        {
            await _userManager.ResetAccessFailedCountAsync(user);
            _logger.LogInformation("User was not locked out. Failed count reset. UserId={UserId}", userId);
            return Result.Ok();
        }

        var lockoutResult = await _userManager.SetLockoutEndDateAsync(user, null);
        if (!lockoutResult.Succeeded)
        {
            var error = string.Join(", ", lockoutResult.Errors.Select(e => e.Description));
            _logger.LogWarning("Unlock failed. UserId={UserId}, Errors={Errors}", userId, error);
            return Result.Fail(error);
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        _logger.LogInformation("User unlocked. UserId={UserId}", userId);
        return Result.Ok();
    }

    public async Task<Result> LockAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Locking user. UserId={UserId}", userId);

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            _logger.LogWarning("Lock failed. User not found. UserId={UserId}", userId);
            return Result.Fail("User not found", HttpStatusCode.NotFound);
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains(RoleNames.SuperAdministrator, StringComparer.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Lock rejected. SuperAdministrator cannot be locked. UserId={UserId}", userId);
            return Result.Fail("SuperAdministrator cannot be locked.", HttpStatusCode.Forbidden);
        }

        var enableResult = await _userManager.SetLockoutEnabledAsync(user, true);
        if (!enableResult.Succeeded)
        {
            var error = string.Join(", ", enableResult.Errors.Select(e => e.Description));
            _logger.LogWarning("Lock failed enabling lockout. UserId={UserId}, Errors={Errors}", userId, error);
            return Result.Fail(error);
        }

        var lockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
        var lockoutResult = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
        if (!lockoutResult.Succeeded)
        {
            var error = string.Join(", ", lockoutResult.Errors.Select(e => e.Description));
            _logger.LogWarning("Lock failed. UserId={UserId}, Errors={Errors}", userId, error);
            return Result.Fail(error);
        }

        _logger.LogInformation("User locked. UserId={UserId}, LockoutEnd={LockoutEnd}", userId, lockoutEnd);
        return Result.Ok();
    }

    public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting user. UserId={UserId}", userId);

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning("Delete failed. User not found. UserId={UserId}", userId);
            return Result.Fail("User not found", HttpStatusCode.NotFound);
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError("User deletion failed. UserId={UserId}, Errors={Errors}", userId, errors);
            return Result.Fail(errors);
        }

        _logger.LogInformation("User deleted successfully. UserId={UserId}", userId);

        return Result.Ok();
    }
}
