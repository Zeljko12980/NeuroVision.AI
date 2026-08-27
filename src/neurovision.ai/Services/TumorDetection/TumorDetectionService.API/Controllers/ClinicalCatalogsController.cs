using MediatR;
using Microsoft.AspNetCore.Mvc;
using TumorDetectionService.Application.ClinicalCatalogs;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.API.Controllers;

[Route("api/tumor/clinical-catalogs")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ClinicalCatalogsController : ControllerBase
{
    private readonly ISender _sender;

    public ClinicalCatalogsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetBundle()
    {
        var result = await _sender.Send(new GetClinicalCatalogsBundleQuery());
        return Ok(result);
    }
}

[Route("api/tumor/tumor-grades")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthPolicies.Staff)]
public class TumorGradesController : ClinicalCatalogCategoryControllerBase
{
    public TumorGradesController(ISender sender) : base(sender, ClinicalCatalogCategory.Grade) { }
}

[Route("api/tumor/operability-statuses")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthPolicies.Staff)]
public class OperabilityStatusesController : ClinicalCatalogCategoryControllerBase
{
    public OperabilityStatusesController(ISender sender) : base(sender, ClinicalCatalogCategory.Operability) { }
}

[Route("api/tumor/spread-statuses")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthPolicies.Staff)]
public class SpreadStatusesController : ClinicalCatalogCategoryControllerBase
{
    public SpreadStatusesController(ISender sender) : base(sender, ClinicalCatalogCategory.Spread) { }
}

[Route("api/tumor/treatment-options")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthPolicies.Staff)]
public class TreatmentOptionsController : ClinicalCatalogCategoryControllerBase
{
    public TreatmentOptionsController(ISender sender) : base(sender, ClinicalCatalogCategory.TreatmentOption) { }
}

public abstract class ClinicalCatalogCategoryControllerBase : ControllerBase
{
    private readonly ISender _sender;
    private readonly ClinicalCatalogCategory _category;

    protected ClinicalCatalogCategoryControllerBase(ISender sender, ClinicalCatalogCategory category)
    {
        _sender = sender;
        _category = category;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 100,
        [FromQuery] string? search = null)
    {
        var result = await _sender.Send(new GetClinicalCatalogByCategoryQuery(_category, pageIndex, pageSize, search));
        return Ok(result);
    }

    [Authorize(Policy = AuthPolicies.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClinicalCatalogItemRequest request)
    {
        var result = await _sender.Send(
            new CreateClinicalCatalogItemCommand(_category, request.Code, request.Name, request.Description));
        return StatusCode(StatusCodes.Status201Created, result);
    }
}

public record CreateClinicalCatalogItemRequest(string Code, string Name, string? Description);
