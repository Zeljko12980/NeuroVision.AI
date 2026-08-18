namespace IdentityService.Application.Common.Requests;

public class AssignRolesRequest
{
    [Required(ErrorMessage = "UserId is required.")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "At least one role must be assigned.")]
    [MinLength(1, ErrorMessage = "At least one role must be assigned.")]
    public List<string> Roles { get; set; }
}
