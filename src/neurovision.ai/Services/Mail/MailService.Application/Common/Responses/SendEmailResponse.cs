namespace MailService.Application.Common.Responses
{
    public class SendEmailResponse
    {
        public Guid EmailId { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
