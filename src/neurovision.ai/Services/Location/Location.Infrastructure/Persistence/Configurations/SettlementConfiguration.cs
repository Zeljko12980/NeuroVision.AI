using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class SettlementConfiguration : IEntityTypeConfiguration<Settlement>
{
    public void Configure(EntityTypeBuilder<Settlement> builder)
    {
        builder.ToTable("Settlements");

        builder.HasKey(s => new { s.CountryCode, s.Code })
            .HasName("PK_SETTLEMENT");

        builder.Property(s => s.CountryCode)
            .HasColumnName("CountryCode")
            .HasColumnType("varchar(3)")
            .IsRequired();

        builder.Property(s => s.Code)
            .HasColumnName("Code")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(s => s.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(120)")
            .IsRequired();

        builder.Property(s => s.PostalCode)
            .HasColumnName("PostalCode")
            .HasColumnType("varchar(12)");

        builder.HasOne(s => s.Country)
            .WithMany(c => c.Settlements)
            .HasForeignKey(s => s.CountryCode)
            .HasConstraintName("FK_SETTLEMENT_COUNTRY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.CountryCode)
            .HasDatabaseName("IX_SETTLEMENT_COUNTRY");
    }
}