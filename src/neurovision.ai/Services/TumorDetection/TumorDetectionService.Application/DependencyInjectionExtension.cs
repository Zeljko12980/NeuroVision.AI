namespace TumorDetectionService.Application;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (HasRabbitMq(configuration))
        {
            services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());
            services.AddScoped<ITumorInboxNotifications, TumorInboxNotificationPublisher>();
        }
        else
        {
            services.AddScoped<ITumorInboxNotifications, NoOpTumorInboxNotifications>();
        }

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }

    private static bool HasRabbitMq(IConfiguration configuration) =>
        !string.IsNullOrWhiteSpace(configuration.GetConnectionString("rabbitmq"))
        || !string.IsNullOrWhiteSpace(configuration["MessageBroker:Host"]);
}
