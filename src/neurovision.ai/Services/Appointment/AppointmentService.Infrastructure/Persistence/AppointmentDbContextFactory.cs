using Microsoft.EntityFrameworkCore.Design;

namespace AppointmentService.Infrastructure.Persistence;

public class AppointmentDbContextFactory : IDesignTimeDbContextFactory<AppointmentDbContext>
{
    public AppointmentDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppointmentDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=appointmentdb;Username=postgres;Password=postgres");
        return new AppointmentDbContext(optionsBuilder.Options);
    }
}
