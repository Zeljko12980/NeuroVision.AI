using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Domain.Entities;

public sealed class TumorAnalysis
{
    public Guid Id { get; private set; }
    public Guid BrainScanId { get; private set; }
    public Guid RequestedByUserId { get; private set; }
    public AnalysisStatus Status { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? DetectionRunId { get; private set; }
    public string? ClassificationRunId { get; private set; }
    public string? SegmentationRunId { get; private set; }
    public string? ReportFilePath { get; private set; }
    public string? PdfReportPath { get; private set; }
    public DateTime? PdfGeneratedAt { get; private set; }
    public double? OverallConfidence { get; private set; }

    public BrainScan BrainScan { get; private set; } = null!;
    public ClassificationResult? Classification { get; private set; }
    public SegmentationResult? Segmentation { get; private set; }
    public ManualCorrection? ManualCorrection { get; private set; }
    public AnalysisClinicalFollowUp? ClinicalFollowUp { get; private set; }
    public ICollection<DetectionFinding> Detections { get; private set; } = new List<DetectionFinding>();
    public ICollection<AnalysisComment> Comments { get; private set; } = new List<AnalysisComment>();
    public ICollection<AnalysisErrorLog> ErrorLogs { get; private set; } = new List<AnalysisErrorLog>();

    private TumorAnalysis() { }

    public static TumorAnalysis Create(Guid brainScanId, Guid requestedByUserId)
    {
        if (brainScanId == Guid.Empty)
            throw new ArgumentException("BrainScanId is required.", nameof(brainScanId));

        return new TumorAnalysis
        {
            Id = Guid.NewGuid(),
            BrainScanId = brainScanId,
            RequestedByUserId = requestedByUserId,
            Status = AnalysisStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };
    }

    public void MarkProcessing()
    {
        Status = AnalysisStatus.Processing;
        StartedAt = DateTime.UtcNow;
    }

    public void MarkCompleted(double? overallConfidence, string? reportFilePath)
    {
        Status = AnalysisStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        OverallConfidence = overallConfidence;
        ReportFilePath = reportFilePath;
    }

    public void MarkFailed()
    {
        Status = AnalysisStatus.Failed;
        CompletedAt = DateTime.UtcNow;
    }

    public void MarkCorrected()
    {
        Status = AnalysisStatus.Corrected;
    }

    public void SetModelRuns(string? detectionRunId, string? classificationRunId, string? segmentationRunId)
    {
        DetectionRunId = detectionRunId;
        ClassificationRunId = classificationRunId;
        SegmentationRunId = segmentationRunId;
    }

    public void AttachClassification(ClassificationResult classification) =>
        Classification = classification;

    public void AttachSegmentation(SegmentationResult segmentation) =>
        Segmentation = segmentation;

    public void SetPdfReport(string pdfReportPath)
    {
        PdfReportPath = pdfReportPath;
        PdfGeneratedAt = DateTime.UtcNow;
    }

    public void AttachManualCorrection(ManualCorrection manualCorrection) =>
        ManualCorrection = manualCorrection;

    public void AttachClinicalFollowUp(AnalysisClinicalFollowUp followUp) =>
        ClinicalFollowUp = followUp;
}
