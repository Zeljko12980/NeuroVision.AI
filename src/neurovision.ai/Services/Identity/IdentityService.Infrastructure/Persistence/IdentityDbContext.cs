namespace IdentityService.Infrastructure.Persistence
{
    public class IdentityContext : IdentityDbContext<AspIdentityUser, AspIdentityRole, Guid>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
