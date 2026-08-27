using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using TumorDetectionService.Application.Analyses.Commands.Start;
using TumorDetectionService.Application.Analyses.Queries;
using TumorDetectionService.Application.Reports.Commands;
using TumorDetectionService.Application.Reports.Queries;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.API.Controllers;

[Route("api/tumor/analyses")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AnalysesController : ControllerBase
{
    private readonly ISender _sender;

    public AnalysesController(ISender sender) => _sender = sender;

    [HttpGet("statistics")]
    [Authorize(Policy = AuthPolicies.Staff)]
    public async Task<IActionResult> GetStatistics()
    {
        var result = await _sender.Send(new GetStatisticsQuery());
        return Ok(result);
    }

    [HttpGet("errors")]
    [Authorize(Policy = AuthPolicies.Staff)]
    public async Task<IActionResult> GetErrors([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await _sender.Send(new GetAnalysisErrorLogsQuery(actor, page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await _sender.Send(new GetAnalysisByIdQuery(id, actor));
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] Guid? patientId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] AnalysisStatus? status,
        [FromQuery] bool? archived,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await _sender.Send(
            new SearchAnalysesQuery(actor, patientId, from, to, status, archived, page, pageSize));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Start([FromBody] StartAnalysisRequest request)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await _sender.Send(new StartAnalysisCommand(request.BrainScanId, actor));
        return Ok(result);
    }

    [HttpGet("{id:guid}/images/{kind}")]
    public async Task<IActionResult> GetImage(Guid id, string kind)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var file = await _sender.Send(new GetAnalysisImageQuery(id, kind, actor));
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(file.FilePath, out var contentType))
            contentType = "application/octet-stream";

        return PhysicalFile(file.FilePath, contentType, enableRangeProcessing: true);
    }

    [HttpPost("{id:guid}/report")]
    [Authorize(Policy = AuthPolicies.Staff)]
    public async Task<IActionResult> GenerateReport(Guid id, [FromBody] GenerateReportRequest? request)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await _sender.Send(new GenerateAnalysisReportCommand(
            id,
            actor,
            request?.DoctorName,
            request?.CertificateId));
        return Ok(result);
    }

    [HttpGet("{id:guid}/report")]
    public async Task<IActionResult> DownloadReport(Guid id)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var file = await _sender.Send(new GetAnalysisReportFileQuery(id, actor));
        return PhysicalFile(file.FilePath, "application/pdf", file.DownloadFileName);
    }
}

public record StartAnalysisRequest(Guid BrainScanId);

public record GenerateReportRequest(string? DoctorName, Guid? CertificateId);
