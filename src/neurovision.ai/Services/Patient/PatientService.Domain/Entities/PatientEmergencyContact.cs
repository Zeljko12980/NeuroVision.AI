namespace PatientService.Domain.Entities;

public class PatientEmergencyContact
{
    public Guid PatientId { get; private set; }
    public int SequenceNumber { get; private set; }
    public string FullName { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string RelationshipCode { get; private set; } = null!;

    public Patient Patient { get; private set; } = null!;
    public RelationshipType Relationship { get; private set; } = null!;

    private PatientEmergencyContact()
    {
    }

    public static PatientEmergencyContact Create(
        Guid patientId,
        int sequenceNumber,
        string fullName,
        string phone,
        string relationshipCode)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient id is required.", nameof(patientId));

        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be greater than zero.", nameof(sequenceNumber));

        return new PatientEmergencyContact
        {
            PatientId = patientId,
            SequenceNumber = sequenceNumber,
            FullName = Guard.NotEmpty(fullName, nameof(fullName)),
            Phone = Guard.NotEmpty(phone, nameof(phone)),
            RelationshipCode = Guard.Code(relationshipCode, nameof(relationshipCode))
        };
    }
}
