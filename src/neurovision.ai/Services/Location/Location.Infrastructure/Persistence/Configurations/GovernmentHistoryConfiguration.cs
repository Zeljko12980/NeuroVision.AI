using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class GovernmentHistoryConfiguration : IEntityTypeConfiguration<GovernmentHistory>
{
    public void Configure(EntityTypeBuilder<GovernmentHistory> builder)
    {
        builder.ToTable("GovernmentHistories");

        builder.HasKey(x => new { x.CountryCode, x.SequenceNumber })
            .HasName("PK_GOVERNMENT_HISTORY");

        builder.Property(x => x.CountryCode)
            .HasColumnName("CountryCode")
            .HasColumnType("varchar(3)")
            .IsRequired();

        builder.Property(x => x.SequenceNumber)
            .HasColumnName("SequenceNumber")
            .HasColumnType("numeric(2,0)")
            .IsRequired();

        builder.Property(x => x.GovernmentTypeCode)
            .HasColumnName("GovernmentTypeCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.From)
            .HasColumnName("From")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.To)
            .HasColumnName("To")
            .HasColumnType("timestamp");

        builder.HasOne(x => x.Country)
            .WithMany(c => c.GovernmentHistory)
            .HasForeignKey(x => x.CountryCode)
            .HasConstraintName("FK_GOVERNMENT_HISTORY_COUNTRY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.GovernmentType)
            .WithMany(g => g.History)
            .HasForeignKey(x => x.GovernmentTypeCode)
            .HasConstraintName("FK_GOVERNMENT_HISTORY_GOVERNMENT_TYPE")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CountryCode)
            .HasDatabaseName("IX_GOVERNMENT_HISTORY_COUNTRY");

        builder.HasIndex(x => x.GovernmentTypeCode)
            .HasDatabaseName("IX_GOVERNMENT_HISTORY_GOVERNMENT_TYPE");
    }
}