namespace PatientService.Application.Common.Response;

public class PatientEmergencyContactResponse
{
    public Guid PatientId { get; set; }
    public int SequenceNumber { get; set; }
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string RelationshipCode { get; set; } = null!;
}
