namespace DoctorService.Domain.Entities;

public class DoctorReview
{
    public Guid DoctorId { get; private set; }
    public int SequenceNumber { get; private set; }
    public decimal Rating { get; private set; }
    public string? Comment { get; private set; }
    public Guid? ReviewerUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Doctor Doctor { get; private set; } = null!;

    private DoctorReview()
    {
    }

    public static DoctorReview Create(
        Guid doctorId,
        int sequenceNumber,
        decimal rating,
        string? comment,
        Guid? reviewerUserId,
        DateTime createdAt)
    {
        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor id is required.", nameof(doctorId));

        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be greater than zero.", nameof(sequenceNumber));

        if (rating is < 1 or > 5)
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));

        return new DoctorReview
        {
            DoctorId = doctorId,
            SequenceNumber = sequenceNumber,
            Rating = decimal.Round(rating, 1),
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
            ReviewerUserId = reviewerUserId,
            CreatedAt = createdAt
        };
    }
}
