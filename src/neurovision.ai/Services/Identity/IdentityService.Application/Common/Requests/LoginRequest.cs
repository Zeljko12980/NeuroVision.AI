namespace IdentityService.Application.Common.Requests;

public class LoginRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; init; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; init; }
}
