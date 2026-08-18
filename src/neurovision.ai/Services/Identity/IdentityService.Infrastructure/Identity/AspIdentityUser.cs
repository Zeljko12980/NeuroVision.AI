namespace IdentityService.Infrastructure.Identity;

public class AspIdentityUser : IdentityUser<Guid>
{
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    protected AspIdentityUser()
    {
    }

    public AspIdentityUser(Guid id, string userName, string email)
    {
        Id = id;
        UserName = userName;
        Email = email;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static AspIdentityUser FromDomain(User user)
        => new(user.Id, user.UserName, user.Email)
        {
            TwoFactorEnabled = user.TwoFactorEnabled,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber
        };

    public void MarkUpdated() => UpdatedAtUtc = DateTime.UtcNow;

    public User ToDomain()
        => User.Restore(
            Id,
            UserName ?? string.Empty,
            Email ?? string.Empty,
            EmailConfirmed,
            TwoFactorEnabled,
            CreatedAtUtc,
            UpdatedAtUtc,
            PhoneNumber);
}
