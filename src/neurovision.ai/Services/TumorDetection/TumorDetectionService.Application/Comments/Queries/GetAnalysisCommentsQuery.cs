using BuildingBlocks.Exceptions;
using MediatR;
using TumorDetectionService.Application.Comments.Commands;
using TumorDetectionService.Application.Common;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Domain.Entities;

namespace TumorDetectionService.Application.Comments.Queries;

public record GetAnalysisCommentsQuery(Guid TumorAnalysisId, TumorActor Actor)
    : IRequest<IReadOnlyList<CommentResponse>>;

public class GetAnalysisCommentsQueryHandler
    : IRequestHandler<GetAnalysisCommentsQuery, IReadOnlyList<CommentResponse>>
{
    private readonly ITumorAnalysisRepository _analyses;
    private readonly IAnalysisCommentRepository _comments;

    public GetAnalysisCommentsQueryHandler(
        ITumorAnalysisRepository analyses,
        IAnalysisCommentRepository comments)
    {
        _analyses = analyses;
        _comments = comments;
    }

    public async Task<IReadOnlyList<CommentResponse>> Handle(
        GetAnalysisCommentsQuery request,
        CancellationToken cancellationToken)
    {
        var analysis = await _analyses.GetByIdWithDetailsAsync(request.TumorAnalysisId, cancellationToken)
            ?? throw new NotFoundException($"Analysis {request.TumorAnalysisId} not found.");
        TumorAccess.EnsureCanAccessAnalysis(request.Actor, analysis);

        var items = await _comments.GetByAnalysisIdAsync(request.TumorAnalysisId, cancellationToken);
        return items.Select(Map).ToList();
    }

    internal static CommentResponse Map(AnalysisComment comment) => new(
        comment.Id,
        comment.TumorAnalysisId,
        comment.AuthorUserId,
        comment.Content,
        comment.CreatedAt,
        comment.UpdatedAt);
}
