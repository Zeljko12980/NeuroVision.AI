namespace IdentityService.Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserDto>> CreateAsync(Guid id, string username, string email);

        Task<Result<UserDto>> GetByEmailAsync(string email);

        Task<Result<string>> GenerateEmailConfirmationTokenAsync(Guid userId);

        Task<Result> DeleteUserAsync(Guid userId);
    }
}
