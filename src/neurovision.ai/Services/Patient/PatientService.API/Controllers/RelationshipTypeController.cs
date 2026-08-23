using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.RelationshipType.Command.Create;
using PatientService.Application.Feature.RelationshipType.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.Staff)]
public class RelationshipTypeController : ControllerBase
{
    private readonly ISender sender;

    public RelationshipTypeController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetRelationshipTypesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllRelationshipTypesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Policy = AuthPolicies.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateRelationshipTypeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateRelationshipTypeCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
