namespace DoctorService.Application.Common.Request;

public sealed class CreateWorkingSlotRequest
{
    public Guid DoctorId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}
