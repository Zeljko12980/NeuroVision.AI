using BuildingBlocks.Persistence;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("notificationdb")
            ?? throw new InvalidOperationException(
                "Connection string 'notificationdb' not found.");

        services.AddDbContext<NotificationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddBuildingBlocksPersistence<NotificationDbContext>(connectionString);

        services.AddScoped<INotificationWriteStore, NotificationWriteStore>();

        return services;
    }
}
