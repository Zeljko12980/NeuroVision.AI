using System.ComponentModel.DataAnnotations;

namespace IdentityService.Application.Common.Requests
{
    public class LoginRequest
    {
        [EmailAddress]
        public string Email { get; init; }
        [Required]
        public string Password { get; init; }
    }
}
