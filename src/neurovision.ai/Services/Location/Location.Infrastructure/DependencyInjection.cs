using BuildingBlocks.Persistence;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;
using LocationService.Infrastructure.Persistence;
using LocationService.Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LocationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("locationdb")
            ?? throw new InvalidOperationException(
                "Connection string 'locationdb' not found.");

        services.AddDbContext<LocationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddBuildingBlocksPersistence<LocationDbContext>(connectionString);

        services.AddScoped<ILocationWriteStore, LocationWriteStore>();
        services.AddScoped(typeof(ILocationReadStore<>), typeof(LocationReadStore<>));
        services.AddScoped(typeof(IRepository<,>), typeof(LocationRepository<,>));

        services.AddSingleton<ILocationSql<CountryResponse>, CountrySql>();
        services.AddSingleton<ILocationSql<GovernmentTypeResponse>, GovernmentTypeSql>();
        services.AddSingleton<ILocationSql<RegionTypeResponse>, RegionTypeSql>();
        services.AddSingleton<ILocationSql<HealthInstitutionTypeResponse>, HealthInstitutionTypeSql>();
        services.AddSingleton<ILocationSql<SettlementResponse>, SettlementSql>();
        services.AddSingleton<ILocationSql<MunicipalityResponse>, MunicipalitySql>();
        services.AddSingleton<ILocationSql<LocalCommunityResponse>, LocalCommunitySql>();
        services.AddSingleton<ILocationSql<CapitalResponse>, CapitalSql>();
        services.AddSingleton<ILocationSql<GovernmentHistoryResponse>, GovernmentHistorySql>();
        services.AddSingleton<ILocationSql<MunicipalitySettlementCoverageResponse>, MunicipalitySettlementCoverageSql>();
        services.AddSingleton<ILocationSql<LocalCommunityCoverageResponse>, LocalCommunityCoverageSql>();
        services.AddSingleton<ILocationSql<RegionResponse>, RegionSql>();
        services.AddSingleton<ILocationSql<RegionSettlementCoverageResponse>, RegionSettlementCoverageSql>();
        services.AddSingleton<ILocationSql<RegionCompositionResponse>, RegionCompositionSql>();
        services.AddSingleton<ILocationSql<CountryCompositionResponse>, CountryCompositionSql>();
        services.AddSingleton<ILocationSql<LegalSuccessorResponse>, LegalSuccessorSql>();
        services.AddSingleton<ILocationSql<HealthInstitutionResponse>, HealthInstitutionSql>();

        return services;
    }
}
