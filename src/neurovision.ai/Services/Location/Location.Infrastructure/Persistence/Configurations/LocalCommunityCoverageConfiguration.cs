using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class LocalCommunityCoverageConfiguration : IEntityTypeConfiguration<LocalCommunityCoverage>
{
    public void Configure(EntityTypeBuilder<LocalCommunityCoverage> builder)
    {
        builder.ToTable("LocalCommunityCoverages");

        builder.HasKey(x => new
        {
            x.MunicipalityCode,
            x.LocalCommunityIdentifier,
            x.CountryCode,
            x.SettlementCode
        })
        .HasName("PK_LOCAL_COMMUNITY_COVERAGE");

        builder.Property(x => x.CountryCode)
            .HasColumnName("CountryCode")
            .HasColumnType("varchar(3)")
            .IsRequired();

        builder.Property(x => x.MunicipalityCode)
            .HasColumnName("MunicipalityCode")
            .HasColumnType("numeric(3,0)")
            .IsRequired();

        builder.Property(x => x.LocalCommunityIdentifier)
            .HasColumnName("LocalCommunityIdentifier")
            .HasColumnType("numeric(2,0)")
            .IsRequired();

        builder.Property(x => x.SettlementCode)
            .HasColumnName("SettlementCode")
            .HasColumnType("int")
            .IsRequired();

        builder.HasOne(x => x.LocalCommunity)
            .WithMany(l => l.Coverages)
            .HasForeignKey(x => new
            {
                x.CountryCode,
                x.MunicipalityCode,
                x.LocalCommunityIdentifier
            })
            .HasPrincipalKey(l => new
            {
                l.CountryCode,
                l.MunicipalityCode,
                l.Identifier
            })
            .HasConstraintName("FK_LOCAL_COMMUNITY_COVERAGE_LOCAL_COMMUNITY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Settlement)
            .WithMany(s => s.LocalCommunityCoverages)
            .HasForeignKey(x => new
            {
                x.CountryCode,
                x.SettlementCode
            })
            .HasPrincipalKey(s => new
            {
                s.CountryCode,
                s.Code
            })
            .HasConstraintName("FK_LOCAL_COMMUNITY_COVERAGE_SETTLEMENT")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.CountryCode,
            x.SettlementCode
        })
        .HasDatabaseName("IX_LOCAL_COMMUNITY_COVERAGE_SETTLEMENT");

        builder.HasIndex(x => new
        {
            x.CountryCode,
            x.MunicipalityCode,
            x.LocalCommunityIdentifier
        })
        .HasDatabaseName("IX_LOCAL_COMMUNITY_COVERAGE_LOCAL_COMMUNITY");
    }
}