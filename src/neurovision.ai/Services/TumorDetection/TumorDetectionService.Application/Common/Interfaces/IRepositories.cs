using TumorDetectionService.Domain.Entities;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Application.Common.Interfaces;

public interface IBrainScanRepository
{
    Task<BrainScan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BrainScan?> GetByIdForProcessingAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<BrainScan> Items, int Total)> GetByPatientAsync(
        Guid? patientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task AddAsync(BrainScan scan, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}

public interface ITumorAnalysisRepository
{
    Task<TumorAnalysis?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<TumorAnalysis> Items, int Total)> SearchAsync(
        Guid? patientId,
        DateTime? from,
        DateTime? to,
        AnalysisStatus? status,
        bool? archived,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task AddAsync(TumorAnalysis analysis, CancellationToken cancellationToken = default);
    Task ApplyPipelineResultsAsync(
        Guid analysisId,
        MlPipelineResult result,
        TumorClassType? predictedClass,
        CancellationToken cancellationToken = default);
    Task ApplyFailureAsync(
        Guid analysisId,
        string message,
        string? details,
        CancellationToken cancellationToken = default);
    Task<int> CountCompletedAsync(CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<TumorAnalysis> Items, int Total)> SearchReportsAsync(
        Guid? patientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

public interface IScanStorageService
{
    Task<string> SaveScanAsync(
        Stream content,
        string fileName,
        Guid scanId,
        CancellationToken cancellationToken = default);
}

public interface IMlAnalysisService
{
    Task<MlPipelineResult> RunPipelineAsync(
        string imagePath,
        string? detectionRun,
        string? classificationRun,
        string? segmentationRun,
        CancellationToken cancellationToken = default);
}

public record MlDetectionDto(string ClassName, double Confidence, double XCenter, double YCenter, double Width, double Height);

public record MlPipelineResult(
    IReadOnlyList<MlDetectionDto> Detections,
    string? ClassificationClass,
    double ClassificationConfidence,
    string ProbabilitiesJson,
    double TumorAreaRatio,
    string? MaskFilePath,
    string? AnnotatedImagePath,
    string? ReportFilePath,
    double OverallConfidence);

public interface IAiModelVersionRepository
{
    Task AddAsync(AiModelVersion version, CancellationToken cancellationToken = default);
    Task<AiModelVersion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AiModelVersion?> GetActiveAsync(AiTaskType taskType, CancellationToken cancellationToken = default);
    Task<AiModelVersion?> GetByRunIdAsync(string runId, CancellationToken cancellationToken = default);
    Task DeactivateAllAsync(AiTaskType taskType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AiModelVersion>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IAiModelTypeRepository
{
    Task AddAsync(AiModelType type, CancellationToken cancellationToken = default);
    Task<AiModelType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<AiModelType> Items, int Total)> SearchAsync(
        string? search,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
}

public interface IAnalysisCommentRepository
{
    Task<AnalysisComment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AnalysisComment>> GetByAnalysisIdAsync(
        Guid analysisId,
        CancellationToken cancellationToken = default);
    Task AddAsync(AnalysisComment comment, CancellationToken cancellationToken = default);
    void Delete(AnalysisComment comment);
}

public interface IAnalysisErrorLogRepository
{
    Task<(IReadOnlyList<AnalysisErrorLog> Items, int Total)> GetRecentAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

public interface IClinicalCatalogRepository
{
    Task AddAsync(ClinicalCatalogItem item, CancellationToken cancellationToken = default);
    Task<ClinicalCatalogItem?> GetAsync(
        ClinicalCatalogCategory category,
        string code,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClinicalCatalogItem>> GetByCategoryAsync(
        ClinicalCatalogCategory category,
        CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<ClinicalCatalogItem> Items, int Total)> SearchByCategoryAsync(
        ClinicalCatalogCategory category,
        string? search,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
}

public interface IAnalysisClinicalFollowUpRepository
{
    Task<AnalysisClinicalFollowUp?> GetByAnalysisIdAsync(
        Guid analysisId,
        CancellationToken cancellationToken = default);
    Task AddAsync(AnalysisClinicalFollowUp followUp, CancellationToken cancellationToken = default);
}

public interface IModelStorageService
{
    Task<string> SaveWeightsAsync(
        AiTaskType taskType,
        string runId,
        Stream content,
        CancellationToken cancellationToken = default);
}
