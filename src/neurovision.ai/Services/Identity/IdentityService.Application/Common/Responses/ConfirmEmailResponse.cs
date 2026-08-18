namespace IdentityService.Application.Common.Responses
{
    public class ConfirmEmailResponse
    {
        public bool IsConfirmed { get; set; }
        public string Message { get; set; } = string.Empty;

    }
}
