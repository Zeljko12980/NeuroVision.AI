namespace IdentityService.Application.Common.Responses
{
    public class SignInResponse
    {
        public bool IsSignedIn { get; set; }
        public string Message { get; set; } = string.Empty;

    }
}
