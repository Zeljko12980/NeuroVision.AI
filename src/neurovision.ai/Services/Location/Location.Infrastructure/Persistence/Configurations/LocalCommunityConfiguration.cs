using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class LocalCommunityConfiguration : IEntityTypeConfiguration<LocalCommunity>
{
    public void Configure(EntityTypeBuilder<LocalCommunity> builder)
    {
        builder.ToTable("LocalCommunities");

        builder.HasKey(l => new { l.CountryCode, l.MunicipalityCode, l.Identifier })
            .HasName("PK_LOCAL_COMMUNITY");

        builder.Property(l => l.CountryCode)
            .HasColumnName("CountryCode")
            .HasColumnType("varchar(3)")
            .IsRequired();

        builder.Property(l => l.MunicipalityCode)
            .HasColumnName("MunicipalityCode")
            .HasColumnType("numeric(3,0)")
            .IsRequired();

        builder.Property(l => l.Identifier)
            .HasColumnName("Identifier")
            .HasColumnType("numeric(2,0)")
            .IsRequired();

        builder.Property(l => l.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(120)")
            .IsRequired();

        builder.Property(l => l.OfficeSettlementCode)
            .HasColumnName("OfficeSettlementCode")
            .HasColumnType("int");

        builder.HasOne(l => l.Municipality)
            .WithMany(m => m.LocalCommunities)
            .HasForeignKey(l => new { l.CountryCode, l.MunicipalityCode })
            .HasPrincipalKey(m => new { m.CountryCode, m.Code })
            .HasConstraintName("FK_LOCAL_COMMUNITY_MUNICIPALITY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.OfficeSettlement)
            .WithMany(s => s.LocalCommunityOffices)
            .HasForeignKey(l => new
            {
                l.CountryCode,
                SettlementCode = l.OfficeSettlementCode
            })
            .HasPrincipalKey(s => new
            {
                s.CountryCode,
                s.Code
            })
            .HasConstraintName("FK_LOCAL_COMMUNITY_OFFICE_SETTLEMENT")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => new { l.CountryCode, l.MunicipalityCode })
            .HasDatabaseName("IX_LOCAL_COMMUNITY_MUNICIPALITY");

        builder.HasIndex(l => new { l.CountryCode, l.OfficeSettlementCode })
            .HasDatabaseName("IX_LOCAL_COMMUNITY_OFFICE_SETTLEMENT");
    }
}