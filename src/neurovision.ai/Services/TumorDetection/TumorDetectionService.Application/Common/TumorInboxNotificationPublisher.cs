using BuildingBlocks.Messaging.Events;
using Microsoft.Extensions.Logging;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Domain.Entities;

namespace TumorDetectionService.Application.Common;

public sealed class TumorInboxNotificationPublisher : ITumorInboxNotifications
{
    public const string TypeCode = "TUMOR";
    public const string RelatedEntityType = "TumorAnalysis";

    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<TumorInboxNotificationPublisher> _logger;

    public TumorInboxNotificationPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<TumorInboxNotificationPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAnalysisOutcomeAsync(
        TumorAnalysis analysis,
        bool succeeded,
        string? errorMessage,
        CancellationToken cancellationToken = default)
    {
        var scanName = analysis.BrainScan.FileName;
        var title = succeeded ? "Tumor analysis completed" : "Tumor analysis failed";
        var message = succeeded
            ? $"AI analysis of scan '{scanName}' has completed."
            : $"AI analysis of scan '{scanName}' failed{(string.IsNullOrWhiteSpace(errorMessage) ? "." : $": {errorMessage}")}";
        var severity = succeeded ? "INFO" : "CRITICAL";

        var recipients = new HashSet<Guid> { analysis.BrainScan.PatientId, analysis.RequestedByUserId };

        foreach (var recipient in recipients)
        {
            try
            {
                await _publishEndpoint.Publish(
                    new CreateNotificationEvent(
                        recipient,
                        TypeCode,
                        severity,
                        title,
                        message,
                        Guid.NewGuid(),
                        RelatedEntityType: RelatedEntityType,
                        RelatedEntityId: analysis.Id),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to publish tumor inbox notification. Recipient={Recipient}, AnalysisId={AnalysisId}",
                    recipient,
                    analysis.Id);
            }
        }
    }
}

internal sealed class NoOpTumorInboxNotifications : ITumorInboxNotifications
{
    public Task PublishAnalysisOutcomeAsync(
        TumorAnalysis analysis,
        bool succeeded,
        string? errorMessage,
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
