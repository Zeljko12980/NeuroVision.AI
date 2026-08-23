using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.PatientAllergyCoverage.Command.Create;
using PatientService.Application.Feature.PatientAllergyCoverage.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientAllergyCoverageController : ControllerBase
{
    private readonly ISender sender;

    public PatientAllergyCoverageController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPatientAllergyCoveragesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllPatientAllergyCoveragesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePatientAllergyCoverageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreatePatientAllergyCoverageCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
