namespace IdentityService.Application.Common.Interfaces;

public interface IUserService
{
    Task<Result<User>> CreateAsync(Guid id, string username, string email, CancellationToken cancellationToken = default);

    Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Result<User>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<User>> UpdateProfileAsync(
        Guid userId,
        string userName,
        string? phoneNumber,
        CancellationToken cancellationToken = default);

    Task<Result<string>> GenerateEmailConfirmationTokenAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<PaginatedResult<UserResponse>>> GetUsersAsync(
        int pageIndex,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default);

    Task<Result> UnlockAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result> LockAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
