using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DoctorService.Application.Common.Request;
using DoctorService.Application.Feature.Specialization.Command.Create;
using DoctorService.Application.Feature.Specialization.Query.GetAll;

namespace DoctorService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpecializationController : ControllerBase
{
    private readonly ISender sender;

    public SpecializationController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetSpecializationsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllSpecializationsQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSpecializationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateSpecializationCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
