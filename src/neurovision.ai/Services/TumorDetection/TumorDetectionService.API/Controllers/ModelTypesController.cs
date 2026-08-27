using MediatR;
using Microsoft.AspNetCore.Mvc;
using TumorDetectionService.Application.ModelTypes;

namespace TumorDetectionService.API.Controllers;

[Route("api/tumor/model-types")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthPolicies.Staff)]
public class ModelTypesController : ControllerBase
{
    private readonly ISender _sender;

    public ModelTypesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 100,
        [FromQuery] string? search = null)
    {
        var result = await _sender.Send(new GetAiModelTypesQuery(pageIndex, pageSize, search));
        return Ok(result);
    }

    [Authorize(Policy = AuthPolicies.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAiModelTypeRequest request)
    {
        var result = await _sender.Send(
            new CreateAiModelTypeCommand(request.Code, request.Name, request.Description));
        return StatusCode(StatusCodes.Status201Created, result);
    }
}

public record CreateAiModelTypeRequest(string Code, string Name, string? Description);
