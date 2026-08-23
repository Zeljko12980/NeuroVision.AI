namespace DoctorService.Application.Common.Response;

public class DoctorStatusHistoryResponse
{
    public Guid DoctorId { get; set; }
    public int SequenceNumber { get; set; }
    public string StatusCode { get; set; } = null!;
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
