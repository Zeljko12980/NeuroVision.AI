namespace IdentityService.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<AspIdentityUser> _userManager;

    public UserService(UserManager<AspIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<User>> CreateAsync(Guid id, string username, string email, CancellationToken cancellationToken = default)
    {
        var user = User.Create(id, username, email);
        var identityUser = AspIdentityUser.FromDomain(user);

        var result = await _userManager.CreateAsync(identityUser);

        if (!result.Succeeded)
        {
            return Result<User>.Fail(
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return Result<User>.Ok(identityUser.ToDomain());
    }

    public async Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return Result<User>.Fail("User not found");

        return Result<User>.Ok(user.ToDomain());
    }

    public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return Result<string>.Fail("User not found");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        return Result<string>.Ok(token);
    }

    public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return Result.Fail("User not found");

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Fail(errors);
        }

        return Result.Ok();
    }
}
