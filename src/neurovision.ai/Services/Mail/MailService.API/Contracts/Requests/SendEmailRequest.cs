using System.ComponentModel.DataAnnotations;

namespace MailService.API.Contracts.Requests
{
    public class SendEmailRequest
    {
        [Required]
        [EmailAddress]
        public string ToEmail { get; set; }
        [Required]
        [MaxLength(255)]
        public string Subject { get; set; }
        [Required]
        [MaxLength(512)]
        public string Body { get; set; } 
    }
}
