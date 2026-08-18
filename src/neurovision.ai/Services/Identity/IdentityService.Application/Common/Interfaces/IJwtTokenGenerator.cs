namespace IdentityService.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string email, string userName, IList<string> roles);
    }
}
