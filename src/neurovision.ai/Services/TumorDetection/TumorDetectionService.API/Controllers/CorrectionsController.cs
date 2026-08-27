using MediatR;
using Microsoft.AspNetCore.Mvc;
using TumorDetectionService.Application.Corrections.Commands;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.API.Controllers;

[Route("api/tumor/analyses/{analysisId:guid}/correction")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthPolicies.Staff)]
public class CorrectionsController : ControllerBase
{
    private readonly ISender _sender;

    public CorrectionsController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Apply(Guid analysisId, [FromBody] ManualCorrectionRequest request)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await _sender.Send(new ApplyManualCorrectionCommand(
            analysisId,
            actor,
            request.CorrectedClass,
            request.Notes));
        return Ok(result);
    }
}

public record ManualCorrectionRequest(TumorClassType CorrectedClass, string? Notes);
