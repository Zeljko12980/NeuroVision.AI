namespace PdfService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.SuperAdmin)]
public class PdfController : ControllerBase
{
    private readonly ISender _sender;

    public PdfController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] PdfTemplateRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreatePdfTemplateCommand(request),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetPdfTemplateByIdQuery(id),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetByCode([FromRoute] string code, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetPdfTemplateByCodeQuery(code),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPdfTemplatesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAllPdfTemplatesQuery(request),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetAllActive(
        [FromQuery] GetPdfTemplatesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetActivePdfTemplatesQuery(request),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdatePdfTemplateRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdatePdfTemplateCommand(id, request),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new DeletePdfTemplateCommand(id),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        [FromBody] Application.Common.Requests.GeneratePdfRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GeneratePdfCommand(request.TemplateCode, request.Data, request.CertificateId, request.UserId),
            cancellationToken);

        return result.ToActionResult();
    }
}
