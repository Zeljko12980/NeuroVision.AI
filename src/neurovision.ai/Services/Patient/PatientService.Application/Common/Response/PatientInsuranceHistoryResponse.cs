namespace PatientService.Application.Common.Response;

public class PatientInsuranceHistoryResponse
{
    public Guid PatientId { get; set; }
    public int SequenceNumber { get; set; }
    public string PayerCode { get; set; } = null!;
    public string PolicyNumber { get; set; } = null!;
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
