namespace IdentityService.Application.Common.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public IReadOnlyList<string> Roles { get; set; } = [];
        public bool IsLockedOut { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }

}
