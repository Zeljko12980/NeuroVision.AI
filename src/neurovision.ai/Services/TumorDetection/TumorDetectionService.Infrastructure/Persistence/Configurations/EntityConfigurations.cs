using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TumorDetectionService.Domain.Entities;

namespace TumorDetectionService.Infrastructure.Persistence.Configurations;

public class DetectionFindingConfiguration : IEntityTypeConfiguration<DetectionFinding>
{
    public void Configure(EntityTypeBuilder<DetectionFinding> builder)
    {
        builder.ToTable("detection_findings");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.ClassName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Confidence).IsRequired();
        builder.Property(x => x.XCenter).IsRequired();
        builder.Property(x => x.YCenter).IsRequired();
        builder.Property(x => x.Width).IsRequired();
        builder.Property(x => x.Height).IsRequired();
    }
}

public class ClassificationResultConfiguration : IEntityTypeConfiguration<ClassificationResult>
{
    public void Configure(EntityTypeBuilder<ClassificationResult> builder)
    {
        builder.ToTable("classification_results");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.PredictedClass).IsRequired();
        builder.Property(x => x.Confidence).IsRequired();
        builder.Property(x => x.ProbabilitiesJson).HasColumnType("jsonb").IsRequired();
    }
}

public class SegmentationResultConfiguration : IEntityTypeConfiguration<SegmentationResult>
{
    public void Configure(EntityTypeBuilder<SegmentationResult> builder)
    {
        builder.ToTable("segmentation_results");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.TumorAreaRatio).IsRequired();
        builder.Property(x => x.MaskFilePath).HasMaxLength(1000);
        builder.Property(x => x.AnnotatedImagePath).HasMaxLength(1000);
    }
}

public class AnalysisCommentConfiguration : IEntityTypeConfiguration<AnalysisComment>
{
    public void Configure(EntityTypeBuilder<AnalysisComment> builder)
    {
        builder.ToTable("analysis_comments");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Content).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasIndex(x => x.TumorAnalysisId);
    }
}

public class ManualCorrectionConfiguration : IEntityTypeConfiguration<ManualCorrection>
{
    public void Configure(EntityTypeBuilder<ManualCorrection> builder)
    {
        builder.ToTable("manual_corrections");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.CorrectedClass).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.Property(x => x.CorrectedAt).IsRequired();
    }
}

public class AiModelTypeConfiguration : IEntityTypeConfiguration<AiModelType>
{
    public void Configure(EntityTypeBuilder<AiModelType> builder)
    {
        builder.ToTable("ai_model_types");
        builder.HasKey(x => x.Code);
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
    }
}

public class AiModelVersionConfiguration : IEntityTypeConfiguration<AiModelVersion>
{
    public void Configure(EntityTypeBuilder<AiModelVersion> builder)
    {
        builder.ToTable("ai_model_versions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.TaskType).IsRequired();
        builder.Property(x => x.VersionLabel).HasMaxLength(100).IsRequired();
        builder.Property(x => x.RunId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.WeightsPath).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.RegisteredAt).IsRequired();

        builder.HasIndex(x => new { x.TaskType, x.IsActive });
        builder.HasIndex(x => x.RunId).IsUnique();
    }
}

public class ClinicalCatalogItemConfiguration : IEntityTypeConfiguration<ClinicalCatalogItem>
{
    public void Configure(EntityTypeBuilder<ClinicalCatalogItem> builder)
    {
        builder.ToTable("clinical_catalog_items");
        builder.HasKey(x => new { x.Category, x.Code });
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
    }
}

public class AnalysisClinicalFollowUpConfiguration : IEntityTypeConfiguration<AnalysisClinicalFollowUp>
{
    public void Configure(EntityTypeBuilder<AnalysisClinicalFollowUp> builder)
    {
        builder.ToTable("analysis_clinical_follow_ups");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.GradeCode).HasMaxLength(50);
        builder.Property(x => x.OperabilityCode).HasMaxLength(50);
        builder.Property(x => x.SpreadCode).HasMaxLength(50);
        builder.Property(x => x.TreatmentOptionCodes).HasMaxLength(500);
        builder.Property(x => x.SizeLocationNotes).HasMaxLength(2000);
        builder.Property(x => x.ClinicalNotes).HasMaxLength(4000);
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasIndex(x => x.TumorAnalysisId).IsUnique();
    }
}

public class AnalysisErrorLogConfiguration : IEntityTypeConfiguration<AnalysisErrorLog>
{
    public void Configure(EntityTypeBuilder<AnalysisErrorLog> builder)
    {
        builder.ToTable("analysis_error_logs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Message).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Details).HasColumnType("text");
        builder.Property(x => x.OccurredAt).IsRequired();
        builder.HasIndex(x => x.OccurredAt);
    }
}
