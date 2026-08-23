namespace PatientService.Application.Common.Request;

public sealed class CreatePatientEmergencyContactRequest
{
    public Guid PatientId { get; set; }
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string RelationshipCode { get; set; } = null!;
}
