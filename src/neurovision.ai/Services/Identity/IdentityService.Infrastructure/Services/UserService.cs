namespace IdentityService.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AspIdentityUser> _userManager;

        public UserService(UserManager<AspIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<UserDto>> CreateAsync(Guid id, string username, string email)
        {
            var identityUser = new AspIdentityUser(id, username, email);

            identityUser.TwoFactorEnabled = true;

            var result = await _userManager.CreateAsync(identityUser);

            if (!result.Succeeded)
            {
                return Result<UserDto>.Fail(
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return Result<UserDto>.Ok(new UserDto
            {
                Id = identityUser.Id,
                UserName = identityUser.UserName,
                Email = identityUser.Email
            });
        }

        public async Task<Result<UserDto>> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Result<UserDto>.Fail("User not found");

            return Result<UserDto>.Ok(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            });
        }

        public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return Result<string>.Fail("User not found");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return Result<string>.Ok(token);
        }

        public async Task<Result> DeleteUserAsync(Guid userId)
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
}
