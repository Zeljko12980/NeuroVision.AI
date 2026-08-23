using Microsoft.EntityFrameworkCore.Design;

namespace PatientService.Infrastructure.Persistence;

public class PatientDbContextFactory : IDesignTimeDbContextFactory<PatientDbContext>
{
    public PatientDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PatientDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=patientdb;Username=postgres;Password=postgres");
        return new PatientDbContext(optionsBuilder.Options);
    }
}
