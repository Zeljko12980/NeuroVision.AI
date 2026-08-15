using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Logging
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddObservabilityTelemetry(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            var options = configuration
                .GetSection(LoggingOptions.SectionName)
                .Get<LoggingOptions>()
                ?? throw new InvalidOperationException("Observability config missing.");

            services.ConfigureOpenTelemetryTracerProvider(provider =>
            {
                provider.SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(options.ServiceName));
            });

            services.ConfigureOpenTelemetryMeterProvider(provider =>
            {
                provider.SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(options.ServiceName));
            });

            return services;
        }
    }
}
