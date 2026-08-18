namespace IdentityService.Application.Common.Requests;

public class CreateRoleRequest
{
    [Required(ErrorMessage = "Role name is required.")]
    [MaxLength(50, ErrorMessage = "Role name must be at most 50 characters long.")]
    [RegularExpression(
        @"^[a-zA-Z0-9_\-\.]+$",
        ErrorMessage = "Role name can only contain letters, numbers, underscore, dash and dot.")]
    public string RoleName { get; set; }

    [MaxLength(250, ErrorMessage = "Description must not exceed 250 characters.")]
    public string? Description { get; set; }
}
