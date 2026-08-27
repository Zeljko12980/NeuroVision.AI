namespace TumorDetectionService.Domain.Entities;

public sealed class AnalysisClinicalFollowUp
{
    public Guid Id { get; private set; }
    public Guid TumorAnalysisId { get; private set; }
    public string? GradeCode { get; private set; }
    public string? OperabilityCode { get; private set; }
    public string? SpreadCode { get; private set; }
    public string? TreatmentOptionCodes { get; private set; }
    public string? SizeLocationNotes { get; private set; }
    public string? ClinicalNotes { get; private set; }
    public Guid UpdatedByUserId { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public TumorAnalysis TumorAnalysis { get; private set; } = null!;

    private AnalysisClinicalFollowUp() { }

    public static AnalysisClinicalFollowUp Create(Guid tumorAnalysisId, Guid updatedByUserId) =>
        new()
        {
            Id = Guid.NewGuid(),
            TumorAnalysisId = tumorAnalysisId,
            UpdatedByUserId = updatedByUserId,
            UpdatedAt = DateTime.UtcNow
        };

    public void Update(
        string? gradeCode,
        string? operabilityCode,
        string? spreadCode,
        IReadOnlyList<string> treatmentOptionCodes,
        string? sizeLocationNotes,
        string? clinicalNotes,
        Guid updatedByUserId)
    {
        GradeCode = NormalizeCode(gradeCode);
        OperabilityCode = NormalizeCode(operabilityCode);
        SpreadCode = NormalizeCode(spreadCode);
        TreatmentOptionCodes = treatmentOptionCodes.Count == 0
            ? null
            : string.Join(',', treatmentOptionCodes
                .Select(NormalizeCode)
                .Where(x => !string.IsNullOrWhiteSpace(x)));
        SizeLocationNotes = NormalizeText(sizeLocationNotes, 2000);
        ClinicalNotes = NormalizeText(clinicalNotes, 4000);
        UpdatedByUserId = updatedByUserId;
        UpdatedAt = DateTime.UtcNow;
    }

    public IReadOnlyList<string> GetTreatmentOptionCodes() =>
        string.IsNullOrWhiteSpace(TreatmentOptionCodes)
            ? Array.Empty<string>()
            : TreatmentOptionCodes
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

    private static string? NormalizeCode(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizeText(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
    }
}
