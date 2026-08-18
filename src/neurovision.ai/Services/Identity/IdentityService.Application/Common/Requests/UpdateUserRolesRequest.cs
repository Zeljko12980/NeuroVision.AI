namespace IdentityService.Application.Common.Requests;

public class UpdateUserRolesRequest
{
    [Required(ErrorMessage = "UserId is required.")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "At least one role must be provided.")]
    [MinLength(1, ErrorMessage = "At least one role must be provided.")]
    public IList<string> Roles { get; set; }
}
