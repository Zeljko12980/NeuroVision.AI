namespace IdentityService.Application.Common.Requests;

public class UpdateProfileRequest
{
    [Required(ErrorMessage = "User name is required.")]
    [MinLength(3, ErrorMessage = "User name must be at least 3 characters long.")]
    [MaxLength(256, ErrorMessage = "User name must be at most 256 characters long.")]
    [RegularExpression(
        @"^[a-zA-Z0-9._@+-]+$",
        ErrorMessage = "User name contains invalid characters.")]
    public string UserName { get; set; }

    [MaxLength(32, ErrorMessage = "Phone number is too long.")]
    public string? PhoneNumber { get; set; }
}
