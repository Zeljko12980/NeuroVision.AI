using LocationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class HealthInstitutionConfiguration : IEntityTypeConfiguration<HealthInstitution>
{
    public void Configure(EntityTypeBuilder<HealthInstitution> builder)
    {
        builder.ToTable("HealthInstitutions");

        builder.HasKey(x => x.Id)
            .HasName("PK_HealthInstitutions");

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(150)")
            .IsRequired();

        builder.Property(x => x.TypeCode)
            .HasColumnName("TypeCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.CountryCode)
            .HasColumnName("CountryCode")
            .HasColumnType("varchar(3)")
            .IsRequired();

        builder.Property(x => x.SettlementCode)
            .HasColumnName("SettlementCode")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(x => x.Address)
            .HasColumnName("Address")
            .HasColumnType("varchar(200)");

        builder.Property(x => x.BedCount)
            .HasColumnName("BedCount")
            .HasColumnType("int");

        builder.Property(x => x.FoundingDate)
            .HasColumnName("FoundingDate")
            .HasColumnType("timestamp");

        builder.Property(x => x.Phone)
            .HasColumnName("Phone")
            .HasColumnType("varchar(30)");

        builder.HasOne(x => x.Type)
            .WithMany(t => t.HealthInstitutions)
            .HasForeignKey(x => x.TypeCode)
            .HasConstraintName("FK_HealthInstitution_HealthInstitutionType")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Country)
            .WithMany(c => c.HealthInstitutions)
            .HasForeignKey(x => x.CountryCode)
            .HasConstraintName("FK_HealthInstitution_Country")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Settlement)
            .WithMany(s => s.HealthInstitutions)
            .HasForeignKey(x => new { x.CountryCode, x.SettlementCode })
            .HasPrincipalKey(s => new { s.CountryCode, s.Code })
            .HasConstraintName("FK_HealthInstitution_Settlement")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TypeCode)
            .HasDatabaseName("IX_HealthInstitution_TypeCode");

        builder.HasIndex(x => x.CountryCode)
            .HasDatabaseName("IX_HealthInstitution_CountryCode");

        builder.HasIndex(x => new { x.CountryCode, x.SettlementCode })
            .HasDatabaseName("IX_HealthInstitution_Settlement");
    }
}