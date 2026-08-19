using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class RegionCompositionConfiguration : IEntityTypeConfiguration<RegionComposition>
{
    public void Configure(EntityTypeBuilder<RegionComposition> builder)
    {
        builder.ToTable("RegionCompositions");

        builder.HasKey(x => new
        {
            x.ParentRegionTypeCode,
            x.ParentRegionCode,
            x.MemberRegionTypeCode,
            x.MemberRegionCode
        })
        .HasName("PK_REGION_COMPOSITION");

        builder.Property(x => x.ParentRegionTypeCode)
            .HasColumnName("ParentRegionTypeCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.ParentRegionCode)
            .HasColumnName("ParentRegionCode")
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(x => x.MemberRegionTypeCode)
            .HasColumnName("MemberRegionTypeCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.MemberRegionCode)
            .HasColumnName("MemberRegionCode")
            .HasColumnType("smallint")
            .IsRequired();

        builder.HasOne(x => x.ParentRegion)
            .WithMany(r => r.AsParentOf)
            .HasForeignKey(x => new
            {
                x.ParentRegionTypeCode,
                x.ParentRegionCode
            })
            .HasPrincipalKey(r => new
            {
                r.TypeCode,
                r.Code
            })
            .HasConstraintName("FK_REGION_COMPOSITION_PARENT_REGION")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.MemberRegion)
            .WithMany(r => r.AsMemberOf)
            .HasForeignKey(x => new
            {
                x.MemberRegionTypeCode,
                x.MemberRegionCode
            })
            .HasPrincipalKey(r => new
            {
                r.TypeCode,
                r.Code
            })
            .HasConstraintName("FK_REGION_COMPOSITION_MEMBER_REGION")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.MemberRegionTypeCode,
            x.MemberRegionCode
        })
        .HasDatabaseName("IX_REGION_COMPOSITION_MEMBER_REGION");

        builder.HasIndex(x => new
        {
            x.ParentRegionTypeCode,
            x.ParentRegionCode
        })
        .HasDatabaseName("IX_REGION_COMPOSITION_PARENT_REGION");
    }
}