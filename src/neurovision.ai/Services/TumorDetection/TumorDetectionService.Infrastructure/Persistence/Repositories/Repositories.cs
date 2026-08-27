using BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Domain.Entities;
using TumorDetectionService.Domain.Enums;
using TumorDetectionService.Infrastructure.Persistence;

namespace TumorDetectionService.Infrastructure.Persistence.Repositories;

public class BrainScanRepository : IBrainScanRepository
{
    private readonly TumorDetectionDbContext _context;

    public BrainScanRepository(TumorDetectionDbContext context) => _context = context;

    public async Task<BrainScan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.BrainScans
            .Include(x => x.Analyses)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<BrainScan?> GetByIdForProcessingAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.BrainScans
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<BrainScan> Items, int Total)> GetByPatientAsync(
        Guid? patientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.BrainScans.AsQueryable();
        if (patientId.HasValue)
            query = query.Where(x => x.PatientId == patientId.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Include(x => x.Analyses)
            .OrderByDescending(x => x.UploadedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task AddAsync(BrainScan scan, CancellationToken cancellationToken = default) =>
        await _context.BrainScans.AddAsync(scan, cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        await _context.BrainScans.CountAsync(cancellationToken);
}

public class TumorAnalysisRepository : ITumorAnalysisRepository
{
    private readonly TumorDetectionDbContext _context;

    public TumorAnalysisRepository(TumorDetectionDbContext context) => _context = context;

    public async Task<TumorAnalysis?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.TumorAnalyses
            .Include(x => x.BrainScan)
            .Include(x => x.Detections)
            .Include(x => x.Classification)
            .Include(x => x.Segmentation)
            .Include(x => x.ManualCorrection)
            .Include(x => x.ClinicalFollowUp)
            .Include(x => x.Comments)
            .Include(x => x.ErrorLogs)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<TumorAnalysis> Items, int Total)> SearchAsync(
        Guid? patientId,
        DateTime? from,
        DateTime? to,
        AnalysisStatus? status,
        bool? archived,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.TumorAnalyses
            .Include(x => x.BrainScan)
            .Include(x => x.Detections)
            .Include(x => x.Classification)
            .Include(x => x.Segmentation)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(x => x.BrainScan.PatientId == patientId.Value);
        if (from.HasValue)
            query = query.Where(x => x.RequestedAt >= from.Value);
        if (to.HasValue)
            query = query.Where(x => x.RequestedAt <= to.Value);
        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (archived == true)
            query = query.Where(x => x.Status == AnalysisStatus.Completed || x.Status == AnalysisStatus.Corrected);
        else if (archived == false)
            query = query.Where(x => x.Status != AnalysisStatus.Completed && x.Status != AnalysisStatus.Corrected);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.RequestedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task AddAsync(TumorAnalysis analysis, CancellationToken cancellationToken = default) =>
        await _context.TumorAnalyses.AddAsync(analysis, cancellationToken);

    public async Task ApplyPipelineResultsAsync(
        Guid analysisId,
        MlPipelineResult result,
        TumorClassType? predictedClass,
        CancellationToken cancellationToken = default)
    {
        var analysis = await _context.TumorAnalyses
            .FirstOrDefaultAsync(x => x.Id == analysisId, cancellationToken)
            ?? throw new InvalidOperationException($"Analysis {analysisId} not found.");

        foreach (var d in result.Detections)
        {
            await _context.DetectionFindings.AddAsync(
                DetectionFinding.Create(
                    analysisId,
                    d.ClassName,
                    d.Confidence,
                    d.XCenter,
                    d.YCenter,
                    d.Width,
                    d.Height),
                cancellationToken);
        }

        if (predictedClass.HasValue)
        {
            await _context.ClassificationResults.AddAsync(
                ClassificationResult.Create(
                    analysisId,
                    predictedClass.Value,
                    result.ClassificationConfidence,
                    result.ProbabilitiesJson),
                cancellationToken);
        }

        await _context.SegmentationResults.AddAsync(
            SegmentationResult.Create(
                analysisId,
                result.TumorAreaRatio,
                result.MaskFilePath,
                result.AnnotatedImagePath),
            cancellationToken);

        analysis.MarkCompleted(result.OverallConfidence, result.ReportFilePath);
    }

    public async Task ApplyFailureAsync(
        Guid analysisId,
        string message,
        string? details,
        CancellationToken cancellationToken = default)
    {
        var analysis = await _context.TumorAnalyses
            .FirstOrDefaultAsync(x => x.Id == analysisId, cancellationToken)
            ?? throw new InvalidOperationException($"Analysis {analysisId} not found.");

        analysis.MarkFailed();
        await _context.AnalysisErrorLogs.AddAsync(
            AnalysisErrorLog.Create(analysisId, message, details),
            cancellationToken);
    }

    public async Task<int> CountCompletedAsync(CancellationToken cancellationToken = default) =>
        await _context.TumorAnalyses.CountAsync(
            x => x.Status == AnalysisStatus.Completed || x.Status == AnalysisStatus.Corrected,
            cancellationToken);

    public async Task<(IReadOnlyList<TumorAnalysis> Items, int Total)> SearchReportsAsync(
        Guid? patientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.TumorAnalyses
            .Include(x => x.BrainScan)
            .Include(x => x.Classification)
            .Where(x => x.PdfReportPath != null)
            .Where(x => x.Status == AnalysisStatus.Completed || x.Status == AnalysisStatus.Corrected);

        if (patientId.HasValue)
            query = query.Where(x => x.BrainScan.PatientId == patientId.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.PdfGeneratedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}

public class AiModelVersionRepository : IAiModelVersionRepository
{
    private readonly TumorDetectionDbContext _context;

    public AiModelVersionRepository(TumorDetectionDbContext context) => _context = context;

    public async Task AddAsync(AiModelVersion version, CancellationToken cancellationToken = default) =>
        await _context.AiModelVersions.AddAsync(version, cancellationToken);

    public async Task<AiModelVersion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.AiModelVersions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<AiModelVersion?> GetActiveAsync(AiTaskType taskType, CancellationToken cancellationToken = default) =>
        await _context.AiModelVersions
            .FirstOrDefaultAsync(x => x.TaskType == taskType && x.IsActive, cancellationToken);

    public async Task<AiModelVersion?> GetByRunIdAsync(string runId, CancellationToken cancellationToken = default) =>
        await _context.AiModelVersions
            .FirstOrDefaultAsync(x => x.RunId == runId, cancellationToken);

    public async Task DeactivateAllAsync(AiTaskType taskType, CancellationToken cancellationToken = default)
    {
        var active = await _context.AiModelVersions
            .Where(x => x.TaskType == taskType && x.IsActive)
            .ToListAsync(cancellationToken);
        foreach (var model in active)
            model.Deactivate();
    }

    public async Task<IReadOnlyList<AiModelVersion>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.AiModelVersions.OrderByDescending(x => x.RegisteredAt).ToListAsync(cancellationToken);
}

public class AiModelTypeRepository : IAiModelTypeRepository
{
    private readonly TumorDetectionDbContext _context;

    public AiModelTypeRepository(TumorDetectionDbContext context) => _context = context;

    public async Task AddAsync(AiModelType type, CancellationToken cancellationToken = default) =>
        await _context.AiModelTypes.AddAsync(type, cancellationToken);

    public async Task<AiModelType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim().ToLower();
        return await _context.AiModelTypes
            .FirstOrDefaultAsync(x => x.Code.ToLower() == normalized, cancellationToken);
    }

    public async Task<(IReadOnlyList<AiModelType> Items, int Total)> SearchAsync(
        string? search,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AiModelTypes.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                x.Code.Contains(term) ||
                x.Name.Contains(term) ||
                (x.Description != null && x.Description.Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.Name)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}

public class TumorDetectionRepository<TEntity, TId> : Repository<TEntity, TId, TumorDetectionDbContext>
    where TEntity : class
    where TId : notnull
{
    public TumorDetectionRepository(TumorDetectionDbContext context) : base(context) { }
}

public class AnalysisCommentRepository : IAnalysisCommentRepository
{
    private readonly TumorDetectionDbContext _context;

    public AnalysisCommentRepository(TumorDetectionDbContext context) => _context = context;

    public async Task<AnalysisComment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.AnalysisComments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<AnalysisComment>> GetByAnalysisIdAsync(
        Guid analysisId,
        CancellationToken cancellationToken = default) =>
        await _context.AnalysisComments
            .Where(x => x.TumorAnalysisId == analysisId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(AnalysisComment comment, CancellationToken cancellationToken = default) =>
        await _context.AnalysisComments.AddAsync(comment, cancellationToken);

    public void Delete(AnalysisComment comment) => _context.AnalysisComments.Remove(comment);
}

public class AnalysisErrorLogRepository : IAnalysisErrorLogRepository
{
    private readonly TumorDetectionDbContext _context;

    public AnalysisErrorLogRepository(TumorDetectionDbContext context) => _context = context;

    public async Task<(IReadOnlyList<AnalysisErrorLog> Items, int Total)> GetRecentAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AnalysisErrorLogs.AsNoTracking();
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.OccurredAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}

public class ClinicalCatalogRepository : IClinicalCatalogRepository
{
    private readonly TumorDetectionDbContext _context;

    public ClinicalCatalogRepository(TumorDetectionDbContext context) => _context = context;

    public async Task AddAsync(ClinicalCatalogItem item, CancellationToken cancellationToken = default) =>
        await _context.ClinicalCatalogItems.AddAsync(item, cancellationToken);

    public async Task<ClinicalCatalogItem?> GetAsync(
        ClinicalCatalogCategory category,
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim();
        return await _context.ClinicalCatalogItems
            .FirstOrDefaultAsync(
                x => x.Category == category && x.Code == normalized,
                cancellationToken);
    }

    public async Task<IReadOnlyList<ClinicalCatalogItem>> GetByCategoryAsync(
        ClinicalCatalogCategory category,
        CancellationToken cancellationToken = default) =>
        await _context.ClinicalCatalogItems
            .Where(x => x.Category == category)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public async Task<(IReadOnlyList<ClinicalCatalogItem> Items, int Total)> SearchByCategoryAsync(
        ClinicalCatalogCategory category,
        string? search,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ClinicalCatalogItems
            .Where(x => x.Category == category);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(x =>
                x.Code.ToLower().Contains(term) ||
                x.Name.ToLower().Contains(term) ||
                (x.Description != null && x.Description.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.Name)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}

public class AnalysisClinicalFollowUpRepository : IAnalysisClinicalFollowUpRepository
{
    private readonly TumorDetectionDbContext _context;

    public AnalysisClinicalFollowUpRepository(TumorDetectionDbContext context) => _context = context;

    public async Task<AnalysisClinicalFollowUp?> GetByAnalysisIdAsync(
        Guid analysisId,
        CancellationToken cancellationToken = default) =>
        await _context.AnalysisClinicalFollowUps
            .FirstOrDefaultAsync(x => x.TumorAnalysisId == analysisId, cancellationToken);

    public async Task AddAsync(AnalysisClinicalFollowUp followUp, CancellationToken cancellationToken = default) =>
        await _context.AnalysisClinicalFollowUps.AddAsync(followUp, cancellationToken);
}
