using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                {
                    config.AddConsumers(assembly);
                }

                config.UsingRabbitMq((context, cfg) =>
                {
                    // Aspire provides ConnectionStrings:rabbitmq
                    var connectionString = configuration.GetConnectionString("rabbitmq");

                    if (!string.IsNullOrWhiteSpace(connectionString))
                    {
                        cfg.Host(new Uri(connectionString));
                    }
                    else
                    {
                        // Fallback for local development
                        var host = configuration["MessageBroker:Host"];
                        var username = configuration["MessageBroker:UserName"];
                        var password = configuration["MessageBroker:Password"];

                        if (string.IsNullOrWhiteSpace(host))
                        {
                            throw new InvalidOperationException(
                                "RabbitMQ configuration not found. Configure either " +
                                "'ConnectionStrings:rabbitmq' (Aspire) or " +
                                "'MessageBroker:Host', 'MessageBroker:UserName', and 'MessageBroker:Password'.");
                        }

                        cfg.Host(new Uri(host), h =>
                        {
                            if (!string.IsNullOrWhiteSpace(username))
                                h.Username(username);

                            if (!string.IsNullOrWhiteSpace(password))
                                h.Password(password);
                        });
                    }

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}