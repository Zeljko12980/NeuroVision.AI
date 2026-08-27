using MediatR;
using Microsoft.AspNetCore.Mvc;
using TumorDetectionService.Application.FollowUp;

namespace TumorDetectionService.API.Controllers;

[Route("api/tumor/analyses/{analysisId:guid}/follow-up")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class FollowUpController : ControllerBase
{
    private readonly ISender _sender;

    public FollowUpController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> Get(Guid analysisId)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await _sender.Send(new GetAnalysisClinicalFollowUpQuery(analysisId, actor));
        return Ok(result);
    }

    [Authorize(Policy = AuthPolicies.Staff)]
    [HttpPut]
    public async Task<IActionResult> Upsert(Guid analysisId, [FromBody] UpsertClinicalFollowUpRequest request)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await _sender.Send(new UpsertAnalysisClinicalFollowUpCommand(
            analysisId,
            actor,
            request.GradeCode,
            request.OperabilityCode,
            request.SpreadCode,
            request.TreatmentOptionCodes ?? [],
            request.SizeLocationNotes,
            request.ClinicalNotes));

        return Ok(result);
    }
}

public record UpsertClinicalFollowUpRequest(
    string? GradeCode,
    string? OperabilityCode,
    string? SpreadCode,
    IReadOnlyList<string>? TreatmentOptionCodes,
    string? SizeLocationNotes,
    string? ClinicalNotes);
