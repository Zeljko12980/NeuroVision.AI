using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DoctorService.Application.Common.Request;
using DoctorService.Application.Feature.DoctorLanguageCoverage.Command.Create;
using DoctorService.Application.Feature.DoctorLanguageCoverage.Query.GetAll;

namespace DoctorService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorLanguageCoverageController : ControllerBase
{
    private readonly ISender sender;

    public DoctorLanguageCoverageController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetDoctorLanguageCoveragesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllDoctorLanguageCoveragesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDoctorLanguageCoverageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateDoctorLanguageCoverageCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
