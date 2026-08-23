using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.Condition.Command.Create;
using PatientService.Application.Feature.Condition.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.Staff)]
public class ConditionController : ControllerBase
{
    private readonly ISender sender;

    public ConditionController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetConditionsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllConditionsQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Policy = AuthPolicies.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateConditionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateConditionCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
