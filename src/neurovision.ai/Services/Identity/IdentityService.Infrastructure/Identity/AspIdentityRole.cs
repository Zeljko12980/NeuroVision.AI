namespace IdentityService.Infrastructure.Identity;

public class AspIdentityRole : IdentityRole<Guid>
{
    public string? Description { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    protected AspIdentityRole()
    {
    }

    public AspIdentityRole(Guid id, string name, string? description = null)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static AspIdentityRole FromDomain(Role role)
        => new(role.Id, role.Name, role.Description);

    public Role ToDomain()
        => Role.Restore(Id, Name ?? string.Empty, Description, CreatedAtUtc, UpdatedAtUtc);

    public void SetDescription(string? description)
    {
        Description = description;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
