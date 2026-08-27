using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TumorDetectionService.Domain.Entities;

namespace TumorDetectionService.Infrastructure.Persistence;

public class TumorDetectionDbContext : DbContext
{
    public TumorDetectionDbContext(DbContextOptions<TumorDetectionDbContext> options)
        : base(options)
    {
    }

    public DbSet<BrainScan> BrainScans => Set<BrainScan>();
    public DbSet<TumorAnalysis> TumorAnalyses => Set<TumorAnalysis>();
    public DbSet<DetectionFinding> DetectionFindings => Set<DetectionFinding>();
    public DbSet<ClassificationResult> ClassificationResults => Set<ClassificationResult>();
    public DbSet<SegmentationResult> SegmentationResults => Set<SegmentationResult>();
    public DbSet<AnalysisComment> AnalysisComments => Set<AnalysisComment>();
    public DbSet<ManualCorrection> ManualCorrections => Set<ManualCorrection>();
    public DbSet<AiModelVersion> AiModelVersions => Set<AiModelVersion>();
    public DbSet<AiModelType> AiModelTypes => Set<AiModelType>();
    public DbSet<ClinicalCatalogItem> ClinicalCatalogItems => Set<ClinicalCatalogItem>();
    public DbSet<AnalysisClinicalFollowUp> AnalysisClinicalFollowUps => Set<AnalysisClinicalFollowUp>();
    public DbSet<AnalysisErrorLog> AnalysisErrorLogs => Set<AnalysisErrorLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
