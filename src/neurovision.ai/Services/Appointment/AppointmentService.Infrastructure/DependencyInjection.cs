using BuildingBlocks.Persistence;

namespace AppointmentService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("appointmentdb")
            ?? throw new InvalidOperationException(
                "Connection string 'appointmentdb' not found.");

        services.AddDbContext<AppointmentDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddBuildingBlocksPersistence<AppointmentDbContext>(connectionString);

        services.AddScoped<IAppointmentWriteStore, AppointmentWriteStore>();

        return services;
    }
}
