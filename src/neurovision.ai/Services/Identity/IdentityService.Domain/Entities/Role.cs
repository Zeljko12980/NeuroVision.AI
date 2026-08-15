namespace IdentityService.Domain.Entities;

public sealed class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private Role()
    {
    }

    public static Role Create(Guid id, string name, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Role
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            Name = name,
            Description = description,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public static Role Restore(
        Guid id,
        string name,
        string? description,
        DateTime createdAtUtc,
        DateTime? updatedAtUtc)
    {
        return new Role
        {
            Id = id,
            Name = name,
            Description = description,
            CreatedAtUtc = createdAtUtc,
            UpdatedAtUtc = updatedAtUtc
        };
    }

    public void Update(string name, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        Description = description;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
