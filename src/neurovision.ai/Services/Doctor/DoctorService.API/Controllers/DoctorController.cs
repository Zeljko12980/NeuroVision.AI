using BuildingBlocks.Results;
using DoctorService.Application.Common.Request;
using DoctorService.Application.Feature.Doctor.Command.Create;
using DoctorService.Application.Feature.Doctor.Command.Delete;
using DoctorService.Application.Feature.Doctor.Query.GetAll;
using DoctorService.Application.Feature.Doctor.Query.GetByKey;
using DoctorService.Application.Feature.Doctor.Query.GetCatalogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DoctorService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorController : ControllerBase
{
    private readonly ISender _sender;

    public DoctorController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetDoctorsRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllDoctorsQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("catalogs")]
    public async Task<IActionResult> GetCatalogs(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetDoctorCatalogsQuery(), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByKey([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetDoctorByKeyQuery(id), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateDoctorRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateDoctorCommand(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteDoctorCommand(id), cancellationToken);
        return result.ToActionResult();
    }
}
