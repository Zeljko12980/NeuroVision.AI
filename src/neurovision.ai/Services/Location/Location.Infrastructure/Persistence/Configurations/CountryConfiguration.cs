using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries");

        builder.HasKey(c => c.Code)
            .HasName("PK_COUNTRY");

        builder.Property(c => c.Code)
            .HasColumnName("Code")
            .HasColumnType("varchar(3)")
            .IsRequired();

        builder.Property(c => c.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(120)")
            .IsRequired();

        builder.Property(c => c.FoundingDate)
            .HasColumnName("FoundingDate")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(c => c.CapitalSettlementCode)
            .HasColumnName("CapitalSettlementCode")
            .HasColumnType("int");

        builder.Property(c => c.GovernmentTypeCode)
            .HasColumnName("GovernmentTypeCode")
            .HasColumnType("varchar(10)");

        builder.Property(c => c.CallingCode)
            .HasColumnName("CallingCode")
            .HasColumnType("numeric(5,0)");

        builder.Property(c => c.Anthem)
            .HasColumnName("Anthem")
            .HasColumnType("bytea");

        builder.Property(c => c.CoatOfArms)
            .HasColumnName("CoatOfArms")
            .HasColumnType("bytea");

        builder.Property(c => c.Flag)
            .HasColumnName("Flag")
            .HasColumnType("bytea");

    
        builder.HasOne(c => c.GovernmentType)
            .WithMany(g => g.Countries)
            .HasForeignKey(c => c.GovernmentTypeCode)
            .HasConstraintName("FK_COUNTRY_GOVERNMENT_TYPE")
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(c => c.CapitalSettlement)
            .WithMany()
            .HasForeignKey(c => new
            {
                CountryCode = c.Code,
                SettlementCode = c.CapitalSettlementCode
            })
            .HasPrincipalKey(s => new
            {
                s.CountryCode,
                s.Code
            })
            .HasConstraintName("FK_COUNTRY_CAPITAL_SETTLEMENT")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.Code, c.CapitalSettlementCode })
            .HasDatabaseName("IX_COUNTRY_CAPITAL_SETTLEMENT");

        builder.HasIndex(c => c.GovernmentTypeCode)
            .HasDatabaseName("IX_COUNTRY_GOVERNMENT_TYPE");
    }
}