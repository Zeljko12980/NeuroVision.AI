namespace MailService.Application.Common.Responses
{
    public class EmailStatusResponse
    {
        public Guid EmailId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? FailedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public int RetryCount { get; set; }
        public int MaxRetries { get; set; }
    }
}
