namespace IdentityService.Application.Common.Requests
{
    public class UpdateRoleRequest
    {
        public string RoleName { get; set; }
        public string? Description { get; set; }
    }
}
