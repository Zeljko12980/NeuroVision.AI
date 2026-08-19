using BuildingBlocks.Persistence;
using LocationService.Application.Common.Interfaces;
using LocationService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LocationService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
         this IServiceCollection services,
         IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("locationDb")
                ?? throw new InvalidOperationException(
                    "Connection string 'locationDb' not found.");

            services.AddDbContext<LocationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<ILocationDbContext>(provider => provider.GetRequiredService<LocationDbContext>());


            services.AddBuildingBlocksPersistence<LocationDbContext>(
                connectionString);
            
            services.AddScoped(
                        typeof(IRepository<,>),
                        typeof(LocationRepository<,>));


            services.AddScoped<IUnitOfWork, UnitOfWork<LocationDbContext>>();

            return services;
        }
    }
}
