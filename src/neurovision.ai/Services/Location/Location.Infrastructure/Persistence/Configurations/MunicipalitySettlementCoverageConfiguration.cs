using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class MunicipalitySettlementCoverageConfiguration : IEntityTypeConfiguration<MunicipalitySettlementCoverage>
{
    public void Configure(EntityTypeBuilder<MunicipalitySettlementCoverage> builder)
    {
        builder.ToTable("MunicipalitySettlementCoverages");

        builder.HasKey(x => new { x.MunicipalityCode, x.CountryCode, x.SettlementCode })
            .HasName("PK_MUNICIPALITY_SETTLEMENT_COVERAGE");

        builder.Property(x => x.CountryCode)
            .HasColumnName("CountryCode")
            .HasColumnType("varchar(3)")
            .IsRequired();

        builder.Property(x => x.MunicipalityCode)
            .HasColumnName("MunicipalityCode")
            .HasColumnType("numeric(3,0)")
            .IsRequired();

        builder.Property(x => x.SettlementCode)
            .HasColumnName("SettlementCode")
            .HasColumnType("int")
            .IsRequired();

        builder.HasOne(x => x.Municipality)
            .WithMany(m => m.Settlements)
            .HasForeignKey(x => new { x.CountryCode, x.MunicipalityCode })
            .HasPrincipalKey(m => new { m.CountryCode, m.Code })
            .HasConstraintName("FK_MUNICIPALITY_SETTLEMENT_COVERAGE_MUNICIPALITY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Settlement)
            .WithMany(s => s.MunicipalityCoverages)
            .HasForeignKey(x => new { x.CountryCode, x.SettlementCode })
            .HasPrincipalKey(s => new { s.CountryCode, s.Code })
            .HasConstraintName("FK_MUNICIPALITY_SETTLEMENT_COVERAGE_SETTLEMENT")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.CountryCode, x.SettlementCode })
            .HasDatabaseName("IX_MUNICIPALITY_SETTLEMENT_COVERAGE_SETTLEMENT");

        builder.HasIndex(x => new { x.CountryCode, x.MunicipalityCode })
            .HasDatabaseName("IX_MUNICIPALITY_SETTLEMENT_COVERAGE_MUNICIPALITY");
    }
}