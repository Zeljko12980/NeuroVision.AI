namespace IdentityService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Result<UserResponse>> CreateAsync(string username, string password, string email, string firstName, string lastName)
        {
            var repoResult = await _userRepository.CreateAsync(username, password, email, firstName, lastName);

            if (!repoResult.Succeeded)
                return Result<UserResponse>.Fail(string.Join(", ", repoResult.Errors));

            var user = await _userRepository.GetByIdAsync(repoResult.UserId);
            if (user == null) return Result<UserResponse>.Fail("User not found after creation.");

            var dto = user.Adapt<UserResponse>();
            return Result<UserResponse>.Ok(dto);
        }

        public async Task<Result<OperationResponse>> DeleteAsync(string userId)
        {
            var result = await _userRepository.DeleteAsync(userId);
            if (!result) return Result<OperationResponse>.Fail("User deletion failed.");

            return Result<OperationResponse>.Ok(new OperationResponse { Success = true });
        }

        public async Task<Result<UserResponse>> GetByIdAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return Result<UserResponse>.Fail("User not found.");

            var dto = user.Adapt<UserResponse>();
            return Result<UserResponse>.Ok(dto);
        }

        public async Task<Result<UserResponse>> GetByUserNameAsync(string userName)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user == null) return Result<UserResponse>.Fail("User not found.");

            var dto = user.Adapt<UserResponse>();
            return Result<UserResponse>.Ok(dto);
        }

        public async Task<Result<UserResponse>> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return Result<UserResponse>.Fail("User not found.");

            var dto = user.Adapt<UserResponse>();
            return Result<UserResponse>.Ok(dto);
        }

        public async Task<Result<OperationResponse>> UpdateProfileAsync(string userId, string firstName, string lastName, string email)
        {
            var result = await _userRepository.UpdateProfileAsync(userId, firstName, lastName, email);
            if (!result) return Result<OperationResponse>.Fail("Profile update failed.");

            return Result<OperationResponse>.Ok(new OperationResponse { Success = true });
        }

        public async Task<Result<OperationResponse>> CheckPasswordAsync(User user, string password)
        {
            var result = await _userRepository.CheckPasswordAsync(user, password);
            if (!result) return Result<OperationResponse>.Fail("Password check failed.");

            return Result<OperationResponse>.Ok(new OperationResponse { Success = true });
        }

        public async Task<Result<string>> GetUserIdAsync(string userName)
        {
            var userId = await _userRepository.GetUserIdAsync(userName);
            if (string.IsNullOrEmpty(userId)) return Result<string>.Fail("User ID not found.");

            return Result<string>.Ok(userId);
        }

        public async Task<Result<List<UserResponse>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);
            if (users == null || users.Count == 0)
                return Result<List<UserResponse>>.Fail("No users found.");

            var dto = users.Adapt<List<UserResponse>>();
            return Result<List<UserResponse>>.Ok(dto);
        }

    }
}
