using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DoctorService.Application.Common.Request;
using DoctorService.Application.Feature.DoctorSpecializationCoverage.Command.Create;
using DoctorService.Application.Feature.DoctorSpecializationCoverage.Query.GetAll;

namespace DoctorService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.SuperAdmin)]
public class DoctorSpecializationCoverageController : ControllerBase
{
    private readonly ISender sender;

    public DoctorSpecializationCoverageController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetDoctorSpecializationCoveragesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllDoctorSpecializationCoveragesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDoctorSpecializationCoverageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateDoctorSpecializationCoverageCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
