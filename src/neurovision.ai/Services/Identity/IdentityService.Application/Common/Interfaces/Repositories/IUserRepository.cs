namespace IdentityService.Application.Common.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateAsync(string username, string password, string email, string firstName, string lastName);

        Task<bool> DeleteAsync(string userId);

        Task<User?> GetByIdAsync(string userId);
        Task<User?> GetByUserNameAsync(string userName);
        Task<User?> GetByEmailAsync(string email);

        Task<bool> UpdateProfileAsync(string userId, string firstName, string lastName, string email);

        Task<bool> CheckPasswordAsync(User user, string password);

        Task<string?> GetUserIdAsync(string userName);

        Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
    }
}
