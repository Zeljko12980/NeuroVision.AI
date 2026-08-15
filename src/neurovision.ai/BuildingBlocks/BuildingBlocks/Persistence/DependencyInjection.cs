using BuildingBlocks.Dapper;

namespace BuildingBlocks.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBuildingBlocksPersistence<TContext>(
          this IServiceCollection services,
          string connectionString)
          where TContext : DbContext
        {
            services.AddScoped<IUnitOfWork,
                UnitOfWork<TContext>>();

            services.AddSingleton<ISqlConnectionFactory>(
                new SqlConnectionFactory(connectionString));

            services.AddScoped<ISqlQueryExecutor,
                SqlQueryExecutor>();

            return services;
        }
    }
}
