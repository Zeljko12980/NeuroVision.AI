namespace TumorDetectionService.Application.Common.Interfaces;

public record AnalysisStatusNotification(
    Guid AnalysisId,
    Guid BrainScanId,
    Guid PatientId,
    string Status);

public interface IAnalysisNotificationPublisher
{
    Task PublishAsync(AnalysisStatusNotification notification, CancellationToken cancellationToken = default);
}

public interface IAnalysisJobRunner
{
    void StartAnalysisJob(
        Guid analysisId,
        string scanFilePath,
        string? detectionRun,
        string? classificationRun,
        string? segmentationRun);
}
