public class AspIdentityUser : IdentityUser<Guid>
{
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    protected AspIdentityUser() { }

    public AspIdentityUser(Guid id, string userName, string email)
    {
        Id = id;
        UserName = userName;
        Email = email;

        CreatedAtUtc = DateTime.UtcNow;
    }

    public void MarkUpdated()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}