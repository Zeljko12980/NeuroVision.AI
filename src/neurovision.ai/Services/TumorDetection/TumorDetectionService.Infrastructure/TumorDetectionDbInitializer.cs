using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TumorDetectionService.Infrastructure.Persistence;

namespace TumorDetectionService.Infrastructure;

public static class TumorDetectionDbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TumorDetectionDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TumorDetectionDbContext>>();

        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Migrate failed, applying EnsureCreated for development.");
            await context.Database.EnsureCreatedAsync();
        }

        await PretrainedModelsBootstrap.EnsureDownloadedAsync(configuration, logger);
        await TumorDetectionDataSeeder.SeedModelTypesAsync(context, logger);
        await TumorDetectionDataSeeder.SeedClinicalCatalogsAsync(context, logger);
        await TumorDetectionDataSeeder.SeedAsync(context, configuration, logger);
    }
}