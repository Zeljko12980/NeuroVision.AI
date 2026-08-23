using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.PatientLanguageCoverage.Command.Create;
using PatientService.Application.Feature.PatientLanguageCoverage.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientLanguageCoverageController : ControllerBase
{
    private readonly ISender sender;

    public PatientLanguageCoverageController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPatientLanguageCoveragesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllPatientLanguageCoveragesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePatientLanguageCoverageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreatePatientLanguageCoverageCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
