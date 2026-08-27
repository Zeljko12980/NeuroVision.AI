using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TumorDetectionService.Domain.Entities;

namespace TumorDetectionService.Infrastructure.Persistence.Configurations;

public class BrainScanConfiguration : IEntityTypeConfiguration<BrainScan>
{
    public void Configure(EntityTypeBuilder<BrainScan> builder)
    {
        builder.ToTable("brain_scans");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PatientId).IsRequired();
        builder.Property(x => x.UploadedByUserId).IsRequired();
        builder.Property(x => x.FileName).HasMaxLength(500).IsRequired();
        builder.Property(x => x.StoredFilePath).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ScanType).IsRequired();
        builder.Property(x => x.FileSizeBytes).IsRequired();
        builder.Property(x => x.UploadedAt).IsRequired();

        builder.HasIndex(x => x.PatientId);
        builder.HasIndex(x => x.UploadedAt);

        builder.HasMany(x => x.Analyses)
            .WithOne(x => x.BrainScan)
            .HasForeignKey(x => x.BrainScanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
