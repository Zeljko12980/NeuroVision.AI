namespace IdentityService.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string UserName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? PhoneNumber { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public bool TwoFactorEnabled { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private User()
    {
    }

    public static User Create(Guid id, string userName, string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        return new User
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            UserName = userName,
            Email = email,
            TwoFactorEnabled = true,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public static User Restore(
        Guid id,
        string userName,
        string email,
        bool emailConfirmed,
        bool twoFactorEnabled,
        DateTime createdAtUtc,
        DateTime? updatedAtUtc,
        string? phoneNumber = null)
    {
        return new User
        {
            Id = id,
            UserName = userName,
            Email = email,
            PhoneNumber = phoneNumber,
            EmailConfirmed = emailConfirmed,
            TwoFactorEnabled = twoFactorEnabled,
            CreatedAtUtc = createdAtUtc,
            UpdatedAtUtc = updatedAtUtc
        };
    }
}
