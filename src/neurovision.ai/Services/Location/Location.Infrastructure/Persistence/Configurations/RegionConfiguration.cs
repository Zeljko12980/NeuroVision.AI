using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.ToTable("Regions");

        builder.HasKey(x => new { x.TypeCode, x.Code })
            .HasName("PK_REGION");

        builder.Property(x => x.TypeCode)
            .HasColumnName("TypeCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.Code)
            .HasColumnName("Code")
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(120)")
            .IsRequired();

        builder.Property(x => x.BelongsToCountryCode)
            .HasColumnName("BelongsToCountryCode")
            .HasColumnType("varchar(3)");

        builder.Property(x => x.HeadquartersCountryCode)
            .HasColumnName("HeadquartersCountryCode")
            .HasColumnType("varchar(3)");

        builder.Property(x => x.AdministrativeSeatSettlementCode)
            .HasColumnName("AdministrativeSeatSettlementCode")
            .HasColumnType("int");

        builder.HasOne(x => x.Type)
            .WithMany(t => t.Regions)
            .HasForeignKey(x => x.TypeCode)
            .HasConstraintName("FK_REGION_TYPE")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BelongsToCountry)
            .WithMany(c => c.HomeRegions)
            .HasForeignKey(x => x.BelongsToCountryCode)
            .HasConstraintName("FK_REGION_COUNTRY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AdministrativeSeatSettlement)
            .WithMany(s => s.RegionAdministrativeSeatOf)
            .HasForeignKey(x => new
            {
                x.HeadquartersCountryCode,
                x.AdministrativeSeatSettlementCode
            })
            .HasPrincipalKey(s => new
            {
                s.CountryCode,
                s.Code
            })
            .HasConstraintName("FK_REGION_ADMINISTRATIVE_SEAT_SETTLEMENT")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TypeCode)
            .HasDatabaseName("IX_REGION_TYPE");

        builder.HasIndex(x => x.BelongsToCountryCode)
            .HasDatabaseName("IX_REGION_COUNTRY");

        builder.HasIndex(x => new
        {
            x.HeadquartersCountryCode,
            x.AdministrativeSeatSettlementCode
        })
        .HasDatabaseName("IX_REGION_ADMINISTRATIVE_SEAT_SETTLEMENT");
    }
}