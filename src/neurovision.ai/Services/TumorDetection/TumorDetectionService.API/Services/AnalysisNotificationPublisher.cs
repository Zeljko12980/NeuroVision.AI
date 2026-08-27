using Microsoft.AspNetCore.SignalR;
using TumorDetectionService.API.Hubs;
using TumorDetectionService.Application.Common.Interfaces;

namespace TumorDetectionService.API.Services;

public class AnalysisNotificationPublisher : IAnalysisNotificationPublisher
{
    private readonly IHubContext<TumorAnalysisHub> _hubContext;

    public AnalysisNotificationPublisher(IHubContext<TumorAnalysisHub> hubContext) =>
        _hubContext = hubContext;

    public async Task PublishAsync(
        AnalysisStatusNotification notification,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            analysisId = notification.AnalysisId,
            brainScanId = notification.BrainScanId,
            patientId = notification.PatientId,
            status = notification.Status
        };

        await _hubContext.Clients
            .Group(TumorAnalysisHub.AnalysisGroup(notification.AnalysisId))
            .SendAsync("AnalysisStatusChanged", payload, cancellationToken);

        await _hubContext.Clients
            .Group(TumorAnalysisHub.PatientGroup(notification.PatientId))
            .SendAsync("AnalysisStatusChanged", payload, cancellationToken);

        await _hubContext.Clients
            .Group(TumorAnalysisHub.AllAnalysesGroup)
            .SendAsync("AnalysisStatusChanged", payload, cancellationToken);
    }
}
