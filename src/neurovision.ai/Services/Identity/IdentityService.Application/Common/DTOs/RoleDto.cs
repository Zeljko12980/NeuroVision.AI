namespace IdentityService.Application.Common.DTOs
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public int? UserCount { get; set; }
        public string? Status { get; set; }
    }
}
