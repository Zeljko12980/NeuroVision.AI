using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DoctorService.Application.Common.Request;
using DoctorService.Application.Feature.DegreeType.Command.Create;
using DoctorService.Application.Feature.DegreeType.Query.GetAll;

namespace DoctorService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DegreeTypeController : ControllerBase
{
    private readonly ISender sender;

    public DegreeTypeController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetDegreeTypesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllDegreeTypesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDegreeTypeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateDegreeTypeCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
