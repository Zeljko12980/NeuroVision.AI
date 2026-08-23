namespace DoctorService.Application.Common.Request;

public sealed class CreateDoctorReviewRequest
{
    public Guid DoctorId { get; set; }
    public decimal Rating { get; set; }
    public string? Comment { get; set; }
    public Guid? ReviewerUserId { get; set; }
}
