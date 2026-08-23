using BuildingBlocks.Results;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.Patient.Command.Create;
using PatientService.Application.Feature.Patient.Command.Delete;
using PatientService.Application.Feature.Patient.Query.GetAll;
using PatientService.Application.Feature.Patient.Query.GetByKey;
using PatientService.Application.Feature.Patient.Query.GetCatalogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientController : ControllerBase
{
    private readonly ISender _sender;

    public PatientController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetPatientsRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllPatientsQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("catalogs")]
    public async Task<IActionResult> GetCatalogs(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPatientCatalogsQuery(), cancellationToken);
        return result.ToActionResult();
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByKey([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPatientByKeyQuery(id), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreatePatientRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreatePatientCommand(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeletePatientCommand(id), cancellationToken);
        return result.ToActionResult();
    }
}
