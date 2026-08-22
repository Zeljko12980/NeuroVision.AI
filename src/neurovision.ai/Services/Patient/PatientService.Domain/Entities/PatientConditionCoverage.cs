namespace PatientService.Domain.Entities;

public class PatientConditionCoverage
{
    public Guid PatientId { get; private set; }
    public string ConditionCode { get; private set; } = null!;
    public int? DiagnosedYear { get; private set; }
    public string? Note { get; private set; }

    public Patient Patient { get; private set; } = null!;
    public Condition Condition { get; private set; } = null!;

    private PatientConditionCoverage()
    {
    }

    public static PatientConditionCoverage Create(
        Guid patientId,
        string conditionCode,
        int? diagnosedYear = null,
        string? note = null)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient id is required.", nameof(patientId));

        if (diagnosedYear is < 1900 or > 2100)
            throw new ArgumentException("Diagnosed year is out of range.", nameof(diagnosedYear));

        return new PatientConditionCoverage
        {
            PatientId = patientId,
            ConditionCode = Guard.Code(conditionCode, nameof(conditionCode)),
            DiagnosedYear = diagnosedYear,
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim()
        };
    }
}
