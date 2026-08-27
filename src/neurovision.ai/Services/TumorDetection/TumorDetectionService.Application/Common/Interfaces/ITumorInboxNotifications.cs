namespace TumorDetectionService.Application.Common.Interfaces;

public interface ITumorInboxNotifications
{
    Task PublishAnalysisOutcomeAsync(
        TumorAnalysis analysis,
        bool succeeded,
        string? errorMessage,
        CancellationToken cancellationToken = default);
}
