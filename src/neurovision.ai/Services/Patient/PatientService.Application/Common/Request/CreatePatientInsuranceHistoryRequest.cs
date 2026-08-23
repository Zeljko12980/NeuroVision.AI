namespace PatientService.Application.Common.Request;

public sealed class CreatePatientInsuranceHistoryRequest
{
    public Guid PatientId { get; set; }
    public string PayerCode { get; set; } = null!;
    public string PolicyNumber { get; set; } = null!;
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
