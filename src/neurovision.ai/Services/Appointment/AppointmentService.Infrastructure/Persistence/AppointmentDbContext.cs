namespace AppointmentService.Infrastructure.Persistence;

public class AppointmentDbContext : DbContext
{
    static AppointmentDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AppointmentType> AppointmentTypes => Set<AppointmentType>();
    public DbSet<AppointmentStatus> AppointmentStatuses => Set<AppointmentStatus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppointmentDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
