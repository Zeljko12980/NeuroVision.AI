using MediatR;
using Microsoft.AspNetCore.Mvc;
using TumorDetectionService.Application.Reports.Queries;

namespace TumorDetectionService.API.Controllers;

[Route("api/tumor/reports")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ReportsController : ControllerBase
{
    private readonly ISender _sender;

    public ReportsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] Guid? patientId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await _sender.Send(new SearchAnalysisReportsQuery(actor, patientId, page, pageSize));
        return Ok(result);
    }
}
