namespace PatientService.Application.Common.Response;

public class PatientDoctorAssignmentHistoryResponse
{
    public Guid PatientId { get; set; }
    public int SequenceNumber { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
