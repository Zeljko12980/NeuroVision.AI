namespace IdentityService.Application.Common.Requests
{
    public class UpdateUserRolesRequest
    {
        public Guid UserId { get; set; } = default!;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
