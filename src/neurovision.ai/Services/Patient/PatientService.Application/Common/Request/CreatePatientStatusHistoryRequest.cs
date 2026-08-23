namespace PatientService.Application.Common.Request;

public sealed class CreatePatientStatusHistoryRequest
{
    public Guid PatientId { get; set; }
    public string StatusCode { get; set; } = null!;
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
