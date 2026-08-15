namespace IdentityService.Application.Common.Requests
{
    public class CreateRoleRequest
    {
        public string RoleName { get; set; }
        public string? Description { get; set; }
    }
}
