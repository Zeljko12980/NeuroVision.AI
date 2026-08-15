namespace IdentityService.Application.Common.Interfaces;

public interface IUserService
{
    Task<Result<User>> CreateAsync(Guid id, string username, string email, CancellationToken cancellationToken = default);

    Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Result<string>> GenerateEmailConfirmationTokenAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
