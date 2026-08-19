using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class RegionSettlementCoverageConfiguration : IEntityTypeConfiguration<RegionSettlementCoverage>
{
    public void Configure(EntityTypeBuilder<RegionSettlementCoverage> builder)
    {
        builder.ToTable("RegionSettlementCoverages");

        builder.HasKey(x => new
        {
            x.CountryCode,
            x.SettlementCode,
            x.RegionTypeCode,
            x.RegionCode
        })
        .HasName("PK_REGION_SETTLEMENT_COVERAGE");

        builder.Property(x => x.RegionTypeCode)
            .HasColumnName("RegionTypeCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.RegionCode)
            .HasColumnName("RegionCode")
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(x => x.CountryCode)
            .HasColumnName("CountryCode")
            .HasColumnType("varchar(3)")
            .IsRequired();

        builder.Property(x => x.SettlementCode)
            .HasColumnName("SettlementCode")
            .HasColumnType("int")
            .IsRequired();

        builder.HasOne(x => x.Region)
            .WithMany(r => r.SettlementCoverages)
            .HasForeignKey(x => new
            {
                x.RegionTypeCode,
                x.RegionCode
            })
            .HasPrincipalKey(r => new
            {
                r.TypeCode,
                r.Code
            })
            .HasConstraintName("FK_REGION_SETTLEMENT_COVERAGE_REGION")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Settlement)
            .WithMany(s => s.RegionCoverages)
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
            .HasConstraintName("FK_REGION_SETTLEMENT_COVERAGE_SETTLEMENT")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.RegionTypeCode,
            x.RegionCode
        })
        .HasDatabaseName("IX_REGION_SETTLEMENT_COVERAGE_REGION");

        builder.HasIndex(x => new
        {
            x.CountryCode,
            x.SettlementCode
        })
        .HasDatabaseName("IX_REGION_SETTLEMENT_COVERAGE_SETTLEMENT");
    }
}