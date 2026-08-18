namespace IdentityService.Application.Common.Requests;

public class Confirm2FARequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Code is required.")]
    public string Code { get; set; }
}
