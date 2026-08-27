using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TumorDetectionService.Domain.Entities;

namespace TumorDetectionService.Infrastructure.Persistence.Configurations;

public class TumorAnalysisConfiguration : IEntityTypeConfiguration<TumorAnalysis>
{
    public void Configure(EntityTypeBuilder<TumorAnalysis> builder)
    {
        builder.ToTable("tumor_analyses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BrainScanId).IsRequired();
        builder.Property(x => x.RequestedByUserId).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.RequestedAt).IsRequired();
        builder.Property(x => x.DetectionRunId).HasMaxLength(100);
        builder.Property(x => x.ClassificationRunId).HasMaxLength(100);
        builder.Property(x => x.SegmentationRunId).HasMaxLength(100);
        builder.Property(x => x.ReportFilePath).HasMaxLength(1000);
        builder.Property(x => x.PdfReportPath).HasMaxLength(1000);
        builder.Property(x => x.PdfGeneratedAt);

        builder.HasIndex(x => x.BrainScanId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.RequestedAt);

        builder.HasOne(x => x.Classification)
            .WithOne(x => x.TumorAnalysis)
            .HasForeignKey<ClassificationResult>(x => x.TumorAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Segmentation)
            .WithOne(x => x.TumorAnalysis)
            .HasForeignKey<SegmentationResult>(x => x.TumorAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ManualCorrection)
            .WithOne(x => x.TumorAnalysis)
            .HasForeignKey<ManualCorrection>(x => x.TumorAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ClinicalFollowUp)
            .WithOne(x => x.TumorAnalysis)
            .HasForeignKey<AnalysisClinicalFollowUp>(x => x.TumorAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Detections)
            .WithOne(x => x.TumorAnalysis)
            .HasForeignKey(x => x.TumorAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Comments)
            .WithOne(x => x.TumorAnalysis)
            .HasForeignKey(x => x.TumorAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ErrorLogs)
            .WithOne(x => x.TumorAnalysis)
            .HasForeignKey(x => x.TumorAnalysisId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
