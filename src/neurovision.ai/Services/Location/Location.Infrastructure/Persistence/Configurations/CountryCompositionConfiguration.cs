using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class CountryCompositionConfiguration : IEntityTypeConfiguration<CountryComposition>
{
    public void Configure(EntityTypeBuilder<CountryComposition> builder)
    {
        builder.ToTable("CountryCompositions");

        builder.HasKey(x => new { x.MemberCountryCode, x.UnionCountryCode, x.SequenceNumber })
            .HasName("PK_COUNTRY_COMPOSITION");

        builder.Property(x => x.UnionCountryCode)
            .HasColumnName("UnionCountryCode")
            .HasColumnType("varchar(3)")
            .IsRequired();

        builder.Property(x => x.MemberCountryCode)
            .HasColumnName("MemberCountryCode")
            .HasColumnType("varchar(3)")
            .IsRequired();

        builder.Property(x => x.SequenceNumber)
            .HasColumnName("SequenceNumber")
            .HasColumnType("numeric(2,0)")
            .IsRequired();

        builder.Property(x => x.From)
            .HasColumnName("From")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.To)
            .HasColumnName("To")
            .HasColumnType("timestamp");

        builder.HasOne(x => x.UnionCountry)
            .WithMany(c => c.UnionMembers)
            .HasForeignKey(x => x.UnionCountryCode)
            .HasConstraintName("FK_COUNTRY_COMPOSITION_UNION_COUNTRY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.MemberCountry)
            .WithMany(c => c.MemberOfUnions)
            .HasForeignKey(x => x.MemberCountryCode)
            .HasConstraintName("FK_COUNTRY_COMPOSITION_MEMBER_COUNTRY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.UnionCountryCode)
            .HasDatabaseName("IX_COUNTRY_COMPOSITION_UNION_COUNTRY");

        builder.HasIndex(x => x.MemberCountryCode)
            .HasDatabaseName("IX_COUNTRY_COMPOSITION_MEMBER_COUNTRY");
    }
}