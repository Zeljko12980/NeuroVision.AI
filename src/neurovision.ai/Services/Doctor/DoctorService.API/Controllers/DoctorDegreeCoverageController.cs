using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DoctorService.Application.Common.Request;
using DoctorService.Application.Feature.DoctorDegreeCoverage.Command.Create;
using DoctorService.Application.Feature.DoctorDegreeCoverage.Query.GetAll;

namespace DoctorService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.SuperAdmin)]
public class DoctorDegreeCoverageController : ControllerBase
{
    private readonly ISender sender;

    public DoctorDegreeCoverageController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetDoctorDegreeCoveragesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllDoctorDegreeCoveragesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDoctorDegreeCoverageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateDoctorDegreeCoverageCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
