namespace DoctorService.Application.Common.Response;

public class DoctorReviewResponse
{
    public Guid DoctorId { get; set; }
    public int SequenceNumber { get; set; }
    public decimal Rating { get; set; }
    public string? Comment { get; set; }
    public Guid? ReviewerUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
