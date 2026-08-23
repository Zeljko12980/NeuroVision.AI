namespace DoctorService.Application.Common.Response;

public class WorkingSlotResponse
{
    public Guid DoctorId { get; set; }
    public int DayOfWeek { get; set; }
    public int SequenceNumber { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}
