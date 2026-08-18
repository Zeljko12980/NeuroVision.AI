namespace IdentityService.Application.Common.Responses
{
    public class RoleResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? UserCount { get; set; }
        public string? Status { get; set; }
    }

}
