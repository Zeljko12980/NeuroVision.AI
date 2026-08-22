using Microsoft.EntityFrameworkCore.Design;

namespace DoctorService.Infrastructure.Persistence;

public class DoctorDbContextFactory : IDesignTimeDbContextFactory<DoctorDbContext>
{
    public DoctorDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DoctorDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=doctordb;Username=postgres;Password=postgres");
        return new DoctorDbContext(optionsBuilder.Options);
    }
}
