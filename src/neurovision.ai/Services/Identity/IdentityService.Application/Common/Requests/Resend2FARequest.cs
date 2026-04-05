using System.ComponentModel.DataAnnotations;

namespace IdentityService.Application.Common.Requests
{
    public class Resend2FARequest
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
