using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TumorDetectionService.Infrastructure.Persistence;

public class TumorDetectionDbContextFactory : IDesignTimeDbContextFactory<TumorDetectionDbContext>
{
    public TumorDetectionDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../TumorDetectionService.API"))
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("tumordetectiondb")
            ?? "Host=localhost;Port=5432;Database=tumordetectiondb;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<TumorDetectionDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new TumorDetectionDbContext(optionsBuilder.Options);
    }
}
