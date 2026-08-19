using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class CapitalConfiguration : IEntityTypeConfiguration<Capital>
{
    public void Configure(EntityTypeBuilder<Capital> builder)
    {
        builder.ToTable("Capitals");

        builder.HasKey(x => new { x.CountryCode, x.SettlementCode, x.SequenceNumber })
            .HasName("PK_CAPITAL");

        builder.Property(x => x.CountryCode)
            .HasColumnName("CountryCode")
            .HasColumnType("varchar(3)")
            .IsRequired();

        builder.Property(x => x.SettlementCode)
            .HasColumnName("SettlementCode")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(x => x.SequenceNumber)
            .HasColumnName("SequenceNumber")
            .HasColumnType("numeric(1,0)")
            .IsRequired();

        builder.Property(x => x.From)
            .HasColumnName("From")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.To)
            .HasColumnName("To")
            .HasColumnType("timestamp");

        builder.HasOne(x => x.Country)
            .WithMany(c => c.CapitalHistory)
            .HasForeignKey(x => x.CountryCode)
            .HasConstraintName("FK_CAPITAL_COUNTRY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Settlement)
            .WithMany(s => s.CapitalOf)
            .HasForeignKey(x => new { x.CountryCode, x.SettlementCode })
            .HasPrincipalKey(s => new { s.CountryCode, s.Code })
            .HasConstraintName("FK_CAPITAL_SETTLEMENT")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CountryCode)
            .HasDatabaseName("IX_CAPITAL_COUNTRY");

        builder.HasIndex(x => new { x.CountryCode, x.SettlementCode })
            .HasDatabaseName("IX_CAPITAL_SETTLEMENT");
    }
}