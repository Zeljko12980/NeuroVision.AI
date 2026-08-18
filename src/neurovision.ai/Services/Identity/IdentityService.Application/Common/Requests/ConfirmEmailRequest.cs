namespace IdentityService.Application.Common.Requests;

public class ConfirmEmailRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Confirmation token is required.")]
    public string Token { get; set; }
}
