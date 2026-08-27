using MediatR;
using Microsoft.AspNetCore.Mvc;
using TumorDetectionService.Application.Comments.Commands;
using TumorDetectionService.Application.Comments.Queries;

namespace TumorDetectionService.API.Controllers;

[Route("api/tumor/analyses/{analysisId:guid}/comments")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CommentsController : ControllerBase
{
    private readonly ISender _sender;

    public CommentsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetComments(Guid analysisId)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await _sender.Send(new GetAnalysisCommentsQuery(analysisId, actor));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.Staff)]
    public async Task<IActionResult> Add(Guid analysisId, [FromBody] CommentRequest request)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await _sender.Send(new AddAnalysisCommentCommand(analysisId, actor, request.Content));
        return Ok(result);
    }

    [HttpPut("{commentId:guid}")]
    [Authorize(Policy = AuthPolicies.Staff)]
    public async Task<IActionResult> Update(Guid commentId, [FromBody] CommentRequest request)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await _sender.Send(new UpdateAnalysisCommentCommand(commentId, actor, request.Content));
        return Ok(result);
    }

    [HttpDelete("{commentId:guid}")]
    [Authorize(Policy = AuthPolicies.Staff)]
    public async Task<IActionResult> Delete(Guid commentId)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        await _sender.Send(new DeleteAnalysisCommentCommand(commentId, actor));
        return NoContent();
    }
}

public record CommentRequest(string Content);
