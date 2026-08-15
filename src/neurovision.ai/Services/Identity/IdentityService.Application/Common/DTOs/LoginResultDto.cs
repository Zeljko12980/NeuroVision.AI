namespace IdentityService.Application.Common.DTOs
{
    public class LoginDto
    {
        public string UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string TwoFactorCode { get; set; } = default!;
    }
}
