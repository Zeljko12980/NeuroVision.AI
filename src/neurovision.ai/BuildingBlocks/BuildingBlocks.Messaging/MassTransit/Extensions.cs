namespace BuildingBlocks.Messaging.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMessageBroker(
            this IServiceCollection services,
            IConfiguration configuration,
            Assembly? assembly = null)
        {
            services.AddMassTransit(config =>
            {
                config.SetKebabCaseEndpointNameFormatter();

                if (assembly != null)
                    config.AddConsumers(assembly);

                config.UsingRabbitMq((context, configurator) =>
                {

                    var connection = configuration.GetConnectionString("rabbitmq");

                    configurator.Host(new Uri(connection!));

                    configurator.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}