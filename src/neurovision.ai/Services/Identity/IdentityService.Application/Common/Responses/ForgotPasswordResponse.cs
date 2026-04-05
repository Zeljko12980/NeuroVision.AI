namespace IdentityService.Application.Common.Responses
{
    public class ForgotPasswordResponse
    {
        public string Message { get; set; } = string.Empty;
        public bool EmailSent { get; set; } = false;
    }
}
