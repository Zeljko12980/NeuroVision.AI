using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using TumorDetectionService.Application.Analyses.Queries;
using TumorDetectionService.Application.Scans.Commands.Upload;
using TumorDetectionService.Application.Scans.Queries;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.API.Controllers;

[Route("api/tumor/scans")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ScansController : ControllerBase
{
    private readonly ISender _sender;

    public ScansController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetScans(
        [FromQuery] Guid? patientId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await _sender.Send(new GetBrainScansQuery(actor, patientId, page, pageSize));
        return Ok(result);
    }

    [HttpPost]
    [RequestSizeLimit(52_428_800)]
    public async Task<IActionResult> Upload(
        [FromForm] Guid patientId,
        [FromForm] ScanType scanType,
        IFormFile file)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        if (file is null || file.Length == 0)
            return BadRequest("Scan file is required.");

        await using var stream = file.OpenReadStream();
        var result = await _sender.Send(new UploadBrainScanCommand(
            patientId,
            actor,
            file.FileName,
            file.ContentType,
            scanType,
            file.Length,
            stream));

        return CreatedAtAction(nameof(GetScans), new { patientId }, result);
    }

    [HttpGet("{id:guid}/image")]
    public async Task<IActionResult> GetImage(Guid id)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var file = await _sender.Send(new GetBrainScanImageQuery(id, actor));
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(file.FilePath, out var contentType))
            contentType = "application/octet-stream";

        return PhysicalFile(file.FilePath, contentType, enableRangeProcessing: true);
    }
}
