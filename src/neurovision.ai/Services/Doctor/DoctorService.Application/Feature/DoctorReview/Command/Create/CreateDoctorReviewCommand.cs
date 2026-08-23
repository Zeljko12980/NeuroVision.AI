using System.Net;

namespace DoctorService.Application.Feature.DoctorReview.Command.Create;

public sealed record CreateDoctorReviewCommand(CreateDoctorReviewRequest Request) : ICommand<Result>;

public sealed class CreateDoctorReviewCommandHandler : ICommandHandler<CreateDoctorReviewCommand, Result>
{
    private readonly IDoctorWriteStore writes;
    private readonly ISequenceStore sequences;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateDoctorReviewCommandHandler> logger;

    public CreateDoctorReviewCommandHandler(
        IDoctorWriteStore writes,
        ISequenceStore sequences,
        IUnitOfWork unitOfWork,
        ILogger<CreateDoctorReviewCommandHandler> logger)
    {
        this.writes = writes;
        this.sequences = sequences;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateDoctorReviewCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var sequence = await sequences.NextAsync(
                "DoctorReviews",
                "SequenceNumber",
                cancellationToken,
                ("DoctorId", request.DoctorId));
            var entity = global::DoctorService.Domain.Entities.DoctorReview.Create(request.DoctorId, sequence, request.Rating, request.Comment, request.ReviewerUserId, DateTime.UtcNow);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("DoctorReview created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
