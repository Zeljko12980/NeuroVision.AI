using Microsoft.EntityFrameworkCore;
using LocationService.Domain.Entities;

namespace LocationService.Application.Common.Interfaces;

public interface ILocationDbContext
{
    DbSet<Country> Countries { get; }
    DbSet<GovernmentType> GovernmentTypes { get; }
    DbSet<Settlement> Settlements { get; }
    DbSet<Municipality> Municipalities { get; }
    DbSet<LocalCommunity> LocalCommunities { get; }
    DbSet<Capital> Capitals { get; }
    DbSet<GovernmentHistory> GovernmentHistories { get; }
    DbSet<MunicipalitySettlementCoverage> MunicipalitySettlementCoverages { get; }
    DbSet<LocalCommunityCoverage> LocalCommunityCoverages { get; }
    DbSet<RegionType> RegionTypes { get; }
    DbSet<Region> Regions { get; }
    DbSet<RegionSettlementCoverage> RegionSettlementCoverages { get; }
    DbSet<RegionComposition> RegionCompositions { get; }
    DbSet<CountryComposition> CountryCompositions { get; }
    DbSet<LegalSuccessor> LegalSuccessors { get; }
    DbSet<HealthInstitution> HealthInstitutions { get; }
    DbSet<HealthInstitutionType> HealthInstitutionTypes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
