using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Domain.Entities;

public sealed class ManualCorrection
{
    public Guid Id { get; private set; }
    public Guid TumorAnalysisId { get; private set; }
    public TumorClassType CorrectedClass { get; private set; }
    public string? Notes { get; private set; }
    public Guid CorrectedByUserId { get; private set; }
    public DateTime CorrectedAt { get; private set; }

    public TumorAnalysis TumorAnalysis { get; private set; } = null!;

    private ManualCorrection() { }

    public static ManualCorrection Create(
        Guid tumorAnalysisId,
        TumorClassType correctedClass,
        Guid correctedByUserId,
        string? notes)
    {
        return new ManualCorrection
        {
            Id = Guid.NewGuid(),
            TumorAnalysisId = tumorAnalysisId,
            CorrectedClass = correctedClass,
            CorrectedByUserId = correctedByUserId,
            Notes = notes,
            CorrectedAt = DateTime.UtcNow
        };
    }
}
