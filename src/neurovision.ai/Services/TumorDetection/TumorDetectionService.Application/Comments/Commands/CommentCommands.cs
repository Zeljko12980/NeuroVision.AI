using BuildingBlocks.Exceptions;
using BuildingBlocks.Persistence;
using MediatR;
using TumorDetectionService.Application.Comments.Queries;
using TumorDetectionService.Application.Common;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Domain.Entities;

namespace TumorDetectionService.Application.Comments.Commands;

public record CommentResponse(
    Guid Id,
    Guid TumorAnalysisId,
    Guid AuthorUserId,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record AddAnalysisCommentCommand(
    Guid TumorAnalysisId,
    TumorActor Actor,
    string Content) : IRequest<CommentResponse>;

public record UpdateAnalysisCommentCommand(
    Guid CommentId,
    TumorActor Actor,
    string Content) : IRequest<CommentResponse>;

public record DeleteAnalysisCommentCommand(Guid CommentId, TumorActor Actor) : IRequest;

public class AddAnalysisCommentCommandHandler : IRequestHandler<AddAnalysisCommentCommand, CommentResponse>
{
    private readonly ITumorAnalysisRepository _analyses;
    private readonly IAnalysisCommentRepository _comments;
    private readonly IUnitOfWork _unitOfWork;

    public AddAnalysisCommentCommandHandler(
        ITumorAnalysisRepository analyses,
        IAnalysisCommentRepository comments,
        IUnitOfWork unitOfWork)
    {
        _analyses = analyses;
        _comments = comments;
        _unitOfWork = unitOfWork;
    }

    public async Task<CommentResponse> Handle(AddAnalysisCommentCommand request, CancellationToken cancellationToken)
    {
        var analysis = await _analyses.GetByIdWithDetailsAsync(request.TumorAnalysisId, cancellationToken)
            ?? throw new NotFoundException($"Analysis {request.TumorAnalysisId} not found.");
        TumorAccess.EnsureCanAccessAnalysis(request.Actor, analysis);
        TumorAccess.EnsureStaff(request.Actor);

        var comment = AnalysisComment.Create(request.TumorAnalysisId, request.Actor.UserId, request.Content);
        await _comments.AddAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return GetAnalysisCommentsQueryHandler.Map(comment);
    }
}

public class UpdateAnalysisCommentCommandHandler : IRequestHandler<UpdateAnalysisCommentCommand, CommentResponse>
{
    private readonly IAnalysisCommentRepository _comments;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAnalysisCommentCommandHandler(IAnalysisCommentRepository comments, IUnitOfWork unitOfWork)
    {
        _comments = comments;
        _unitOfWork = unitOfWork;
    }

    public async Task<CommentResponse> Handle(UpdateAnalysisCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _comments.GetByIdAsync(request.CommentId, cancellationToken)
            ?? throw new NotFoundException($"Comment {request.CommentId} not found.");

        TumorAccess.EnsureStaff(request.Actor);

        comment.UpdateContent(request.Content);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return GetAnalysisCommentsQueryHandler.Map(comment);
    }
}

public class DeleteAnalysisCommentCommandHandler : IRequestHandler<DeleteAnalysisCommentCommand>
{
    private readonly IAnalysisCommentRepository _comments;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAnalysisCommentCommandHandler(IAnalysisCommentRepository comments, IUnitOfWork unitOfWork)
    {
        _comments = comments;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteAnalysisCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _comments.GetByIdAsync(request.CommentId, cancellationToken)
            ?? throw new NotFoundException($"Comment {request.CommentId} not found.");

        TumorAccess.EnsureStaff(request.Actor);

        _comments.Delete(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
