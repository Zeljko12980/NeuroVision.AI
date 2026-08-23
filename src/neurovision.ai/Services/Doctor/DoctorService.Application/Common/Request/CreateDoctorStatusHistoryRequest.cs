namespace DoctorService.Application.Common.Request;

public sealed class CreateDoctorStatusHistoryRequest
{
    public Guid DoctorId { get; set; }
    public string StatusCode { get; set; } = null!;
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
