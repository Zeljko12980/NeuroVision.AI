namespace IdentityService.Application.Common.Responses
{
    public class ResetPasswordResponse
    {
        public string Message { get; set; } = string.Empty;
        public bool PasswordReset { get; set; } = false;
    }
}
