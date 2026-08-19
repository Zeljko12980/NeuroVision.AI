using LocationService.Application.Common.Response;
using LocationService.Domain.Entities;
using Mapster;

namespace LocationService.Application.Common.Mapping
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Country, CountryResponse>.NewConfig()
                .Map(
                    dest => dest.GovernmentTypeName,
                    src => src.GovernmentType != null
                        ? src.GovernmentType.Name
                        : null
                )
                .Map(
                    dest => dest.CapitalSettlementName,
                    src => src.CapitalSettlement != null
                        ? src.CapitalSettlement.Name
                        : null
                )
                .Map(
                    dest => dest.SettlementCount,
                    src => src.Settlements.Count
                )
                .Map(
                    dest => dest.MunicipalityCount,
                    src => src.Municipalities.Count
                )
                .Map(
                    dest => dest.HealthInstitutionCount,
                    src => src.HealthInstitutions.Count
                );
        }
    }
}
