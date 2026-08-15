using BuildingBlocks.Results;
using IdentityService.Application.Common.DTOs;

namespace IdentityService.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AspIdentityUser> _userManager;

        public UserService(UserManager<AspIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // =========================
        // CREATE
        // =========================
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

        // =========================
        // GET BY ID
        // =========================
        public async Task<Result<UserDto>> GetByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return Result<UserDto>.Fail("User not found");

            return Result<UserDto>.Ok(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            });
        }

        // =========================
        // GET BY EMAIL
        // =========================
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

        // =========================
        // EMAIL CONFIRM TOKEN
        // =========================
        public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return Result<string>.Fail("User not found");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return Result<string>.Ok(token);
        }

        // =========================
        // SET PASSWORD
        // =========================
        public async Task<Result> SetPasswordAsync(Guid userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return Result.Fail("User not found");

            if (await _userManager.HasPasswordAsync(user))
                return Result.Fail("User already has password");

            var result = await _userManager.AddPasswordAsync(user, password);

            if (!result.Succeeded)
                return Result.Fail(string.Join(", ", result.Errors.Select(e => e.Description)));

            return Result.Ok();
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