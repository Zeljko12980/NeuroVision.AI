using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.Gender.Command.Create;
using PatientService.Application.Feature.Gender.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.Staff)]
public class GenderController : ControllerBase
{
    private readonly ISender sender;

    public GenderController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetGendersRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllGendersQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Policy = AuthPolicies.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateGenderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateGenderCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
