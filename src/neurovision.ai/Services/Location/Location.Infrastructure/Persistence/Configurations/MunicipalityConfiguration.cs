using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class MunicipalityConfiguration : IEntityTypeConfiguration<Municipality>
{
    public void Configure(EntityTypeBuilder<Municipality> builder)
    {
        builder.ToTable("Municipalities");

        builder.HasKey(m => new { m.CountryCode, m.Code })
            .HasName("PK_MUNICIPALITY");

        builder.Property(m => m.CountryCode)
            .HasColumnName("CountryCode")
            .HasColumnType("varchar(3)")
            .IsRequired();

        builder.Property(m => m.Code)
            .HasColumnName("Code")
            .HasColumnType("numeric(3,0)")
            .IsRequired();

        builder.Property(m => m.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(120)")
            .IsRequired();

        builder.Property(m => m.SeatSettlementCode)
            .HasColumnName("SeatSettlementCode")
            .HasColumnType("int");

        builder.HasOne(m => m.Country)
            .WithMany(c => c.Municipalities)
            .HasForeignKey(m => m.CountryCode)
            .HasConstraintName("FK_MUNICIPALITY_COUNTRY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.SeatSettlement)
            .WithMany(s => s.MunicipalitySeatOf)
            .HasForeignKey(m => new
            {
                m.CountryCode,
                SettlementCode = m.SeatSettlementCode
            })
            .HasPrincipalKey(s => new
            {
                s.CountryCode,
                s.Code
            })
            .HasConstraintName("FK_MUNICIPALITY_SEAT_SETTLEMENT")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => m.CountryCode)
            .HasDatabaseName("IX_MUNICIPALITY_COUNTRY");

        builder.HasIndex(m => new
        {
            m.CountryCode,
            m.SeatSettlementCode
        })
        .HasDatabaseName("IX_MUNICIPALITY_SEAT_SETTLEMENT");
    }
}