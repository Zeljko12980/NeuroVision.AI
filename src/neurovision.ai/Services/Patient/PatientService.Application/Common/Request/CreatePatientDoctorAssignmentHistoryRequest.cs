namespace PatientService.Application.Common.Request;

public sealed class CreatePatientDoctorAssignmentHistoryRequest
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
