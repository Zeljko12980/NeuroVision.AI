namespace PatientService.Domain.Entities;

public class PatientAllergyCoverage
{
    public Guid PatientId { get; private set; }
    public string AllergyCode { get; private set; } = null!;
    public string? Note { get; private set; }

    public Patient Patient { get; private set; } = null!;
    public Allergy Allergy { get; private set; } = null!;

    private PatientAllergyCoverage()
    {
    }

    public static PatientAllergyCoverage Create(Guid patientId, string allergyCode, string? note = null)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient id is required.", nameof(patientId));

        return new PatientAllergyCoverage
        {
            PatientId = patientId,
            AllergyCode = Guard.Code(allergyCode, nameof(allergyCode)),
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim()
        };
    }
}
