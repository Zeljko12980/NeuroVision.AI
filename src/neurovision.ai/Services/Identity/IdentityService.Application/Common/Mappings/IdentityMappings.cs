namespace IdentityService.Application.Common.Mappings;

public static class IdentityMappings
{
    public static UserResponse ToResponse(this User user)
        => new()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };

    public static RoleResponse ToResponse(this Role role)
        => new()
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description
        };
}
