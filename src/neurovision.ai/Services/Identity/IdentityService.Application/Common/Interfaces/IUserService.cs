namespace IdentityService.Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserResponse>> CreateAsync(string username, string password, string email, string firstName, string lastName);
        Task<Result<OperationResponse>> DeleteAsync(string userId);
        Task<Result<UserResponse>> GetByIdAsync(string userId);
        Task<Result<UserResponse>> GetByUserNameAsync(string userName);
        Task<Result<UserResponse>> GetByEmailAsync(string email);
        Task<Result<OperationResponse>> UpdateProfileAsync(string userId, string firstName, string lastName, string email);
        Task<Result<OperationResponse>> CheckPasswordAsync(User user, string password);
        Task<Result<string>> GetUserIdAsync(string userName);
        Task<Result<List<UserResponse>>> GetAllAsync(CancellationToken cancellationToken);
    }

}
