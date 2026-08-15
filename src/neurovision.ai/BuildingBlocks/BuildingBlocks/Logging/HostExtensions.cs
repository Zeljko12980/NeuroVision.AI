using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace BuildingBlocks.Logging
{
    public static class HostExtensions
    {
        public static IHostBuilder AddSerilogObservability(this IHostBuilder host)
        {
            host.UseSerilog((context, services, logger) =>
            {
                var options = context.Configuration
                    .GetSection(LoggingOptions.SectionName)
                    .Get<LoggingOptions>()
                    ?? throw new InvalidOperationException("Observability config missing.");

                var serviceName = options.ServiceName ?? context.HostingEnvironment.ApplicationName;

                logger
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("service", serviceName)
                    .Enrich.WithProperty("environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithProperty("traceId", Activity.Current?.TraceId.ToString())
                    .WriteTo.Console()
                    .WriteTo.GrafanaLoki(
                        options.LokiUrl,
                        labels: new[]
                        {
                        new LokiLabel { Key = "service", Value = serviceName },
                        new LokiLabel { Key = "environment", Value = context.HostingEnvironment.EnvironmentName },
                        new LokiLabel { Key = "application", Value = serviceName }
                        });
            });

            return host;
        }
    }
}
