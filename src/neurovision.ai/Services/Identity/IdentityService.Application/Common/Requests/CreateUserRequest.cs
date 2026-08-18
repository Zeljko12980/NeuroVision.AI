namespace IdentityService.Application.Common.Requests;

public class CreateUserRequest
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "User name is required.")]
    [MinLength(3, ErrorMessage = "User name must be at least 3 characters long.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "At least one role is required.")]
    [MinLength(1, ErrorMessage = "At least one role is required.")]
    public List<string> Roles { get; set; }
}
