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

    public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting user. UserId={UserId}", userId);

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning("Delete failed. User not found. UserId={UserId}", userId);
            return Result.Fail("User not found");
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
