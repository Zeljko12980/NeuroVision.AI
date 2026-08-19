using LocationService.Application.Common.Interfaces;
using LocationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocationService.Infrastructure.Persistence;

public class LocationDbContext : DbContext, ILocationDbContext
{
    public LocationDbContext(DbContextOptions<LocationDbContext> options) : base(options)
    {
    }

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<GovernmentType> GovernmentTypes => Set<GovernmentType>();
    public DbSet<Settlement> Settlements => Set<Settlement>();
    public DbSet<Municipality> Municipalities => Set<Municipality>();
    public DbSet<LocalCommunity> LocalCommunities => Set<LocalCommunity>();
    public DbSet<Capital> Capitals => Set<Capital>();
    public DbSet<GovernmentHistory> GovernmentHistories => Set<GovernmentHistory>();
    public DbSet<MunicipalitySettlementCoverage> MunicipalitySettlementCoverages => Set<MunicipalitySettlementCoverage>();
    public DbSet<LocalCommunityCoverage> LocalCommunityCoverages => Set<LocalCommunityCoverage>();
    public DbSet<RegionType> RegionTypes => Set<RegionType>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<RegionSettlementCoverage> RegionSettlementCoverages => Set<RegionSettlementCoverage>();
    public DbSet<RegionComposition> RegionCompositions => Set<RegionComposition>();
    public DbSet<CountryComposition> CountryCompositions => Set<CountryComposition>();
    public DbSet<LegalSuccessor> LegalSuccessors => Set<LegalSuccessor>();

    public DbSet<HealthInstitution> HealthInstitutions => Set<HealthInstitution>();
    public DbSet<HealthInstitutionType> HealthInstitutionTypes => Set<HealthInstitutionType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LocationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
    