using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class LegalSuccessorConfiguration : IEntityTypeConfiguration<LegalSuccessor>
{
    public void Configure(EntityTypeBuilder<LegalSuccessor> builder)
    {
        builder.ToTable("LegalSuccessors");

        builder.HasKey(x => new { x.PredecessorCountryCode, x.SuccessorCountryCode })
            .HasName("PK_LEGAL_SUCCESSOR");

        builder.Property(x => x.SuccessorCountryCode)
            .HasColumnName("SuccessorCountryCode")
            .HasColumnType("varchar(3)")
            .IsRequired();

        builder.Property(x => x.PredecessorCountryCode)
            .HasColumnName("PredecessorCountryCode")
            .HasColumnType("varchar(3)")
            .IsRequired();

        builder.HasOne(x => x.SuccessorCountry)
            .WithMany(c => c.SuccessorOf)
            .HasForeignKey(x => x.SuccessorCountryCode)
            .HasConstraintName("FK_LEGAL_SUCCESSOR_SUCCESSOR_COUNTRY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PredecessorCountry)
            .WithMany(c => c.PredecessorOf)
            .HasForeignKey(x => x.PredecessorCountryCode)
            .HasConstraintName("FK_LEGAL_SUCCESSOR_PREDECESSOR_COUNTRY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.SuccessorCountryCode)
            .HasDatabaseName("IX_LEGAL_SUCCESSOR_SUCCESSOR_COUNTRY");

        builder.HasIndex(x => x.PredecessorCountryCode)
            .HasDatabaseName("IX_LEGAL_SUCCESSOR_PREDECESSOR_COUNTRY");
    }
}