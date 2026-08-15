using System.ComponentModel.DataAnnotations;

namespace IdentityService.Application.Common.Requests
{
    public class Confirm2FARequest
    {
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
