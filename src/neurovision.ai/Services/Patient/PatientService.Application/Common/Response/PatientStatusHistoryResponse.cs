namespace PatientService.Application.Common.Response;

public class PatientStatusHistoryResponse
{
    public Guid PatientId { get; set; }
    public int SequenceNumber { get; set; }
    public string StatusCode { get; set; } = null!;
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
