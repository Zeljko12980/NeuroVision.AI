namespace IdentityService.Application.Common.Requests
{
    public class SetPasswordRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
