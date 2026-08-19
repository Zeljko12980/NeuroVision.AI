using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Mapping;
using LocationService.Application.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LocationService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            MappingConfig.RegisterMappings();

            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IGovernmentTypeService, GovernmentTypeService>();
            services.AddScoped<IRegionTypeService, RegionTypeService>();
            services.AddScoped<IHealthInstitutionTypeService, HealthInstitutionTypeService>();
            services.AddScoped<ISettlementService, SettlementService>();
            services.AddScoped<IMunicipalityService, MunicipalityService>();
            services.AddScoped<ILocalCommunityService, LocalCommunityService>();
            services.AddScoped<ICapitalService, CapitalService>();
            services.AddScoped<IGovernmentHistoryService, GovernmentHistoryService>();
            services.AddScoped<IMunicipalitySettlementCoverageService, MunicipalitySettlementCoverageService>();
            services.AddScoped<ILocalCommunityCoverageService, LocalCommunityCoverageService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<IRegionCompositionService, RegionCompositionService>();
            services.AddScoped<IRegionSettlementCoverageService, RegionSettlementCoverageService>();
            services.AddScoped<ICountryCompositionService, CountryCompositionService>();
            services.AddScoped<ILegalSuccessorService, LegalSuccessorService>();
            services.AddScoped<IHealthInstitutionService, HealthInstitutionService>();

            services.AddMediatR(ctg =>
            {
                ctg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });


            return services;
        }
    }
}
