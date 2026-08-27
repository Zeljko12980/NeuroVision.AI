namespace TumorDetectionService.Domain.Entities;

public sealed class AnalysisErrorLog
{
    public Guid Id { get; private set; }
    public Guid? TumorAnalysisId { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public string? Details { get; private set; }
    public DateTime OccurredAt { get; private set; }

    public TumorAnalysis? TumorAnalysis { get; private set; }

    private AnalysisErrorLog() { }

    public static AnalysisErrorLog Create(Guid? tumorAnalysisId, string message, string? details)
    {
        return new AnalysisErrorLog
        {
            Id = Guid.NewGuid(),
            TumorAnalysisId = tumorAnalysisId,
            Message = message,
            Details = details,
            OccurredAt = DateTime.UtcNow
        };
    }
}
