namespace IdentityService.Application.Common.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
    }

}
