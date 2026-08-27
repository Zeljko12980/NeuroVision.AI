using MediatR;
using Microsoft.AspNetCore.Mvc;
using TumorDetectionService.Application.Models.Commands;
using TumorDetectionService.Application.Models.Queries;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.API.Controllers;

[Route("api/tumor/models")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthPolicies.Staff)]
public class ModelsController : ControllerBase
{
    private readonly ISender _sender;

    public ModelsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sender.Send(new GetAiModelVersionsQuery());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterModelRequest request)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await _sender.Send(new RegisterAiModelVersionCommand(
            request.TaskType,
            request.VersionLabel,
            request.RunId,
            request.WeightsPath,
            actor.UserId,
            request.SetActive));
        return Ok(result);
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var result = await _sender.Send(new ActivateAiModelVersionCommand(id));
        return Ok(result);
    }

    [HttpPost("upload")]
    [RequestSizeLimit(536_870_912)]
    [RequestFormLimits(MultipartBodyLengthLimit = 536_870_912)]
    public async Task<IActionResult> Upload(
        [FromForm] string taskType,
        [FromForm] string versionLabel,
        [FromForm] string? runId,
        [FromForm] bool setActive = true,
        [FromForm] IFormFile? file = null)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        if (file is null || file.Length == 0)
            return BadRequest("Weight file is required.");

        await using var stream = file.OpenReadStream();
        var result = await _sender.Send(new UploadAiModelVersionCommand(
            taskType,
            versionLabel,
            runId,
            stream,
            file.FileName,
            actor.UserId,
            setActive));
        return Ok(result);
    }
}

public record RegisterModelRequest(
    AiTaskType TaskType,
    string VersionLabel,
    string RunId,
    string WeightsPath,
    bool SetActive = true);
