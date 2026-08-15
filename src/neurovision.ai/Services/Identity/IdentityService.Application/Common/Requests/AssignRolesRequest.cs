namespace IdentityService.Application.Common.Requests
{
    public class AssignRolesRequest
    {
        public Guid UserId { get; set; }
        public List<string> Roles { get; set; }
    }
}
