namespace PdfService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.SuperAdmin)]
public class CertificateController : ControllerBase
{
    private readonly ISender _sender;

    public CertificateController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllCertificatesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCertificateByIdQuery(id), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(
        [FromForm] Guid userId,
        [FromForm] string name,
        [FromForm] string? password,
        IFormFile file,
        IFormFile signatureImage,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest("The certificate file is required.");

        if (signatureImage is null || signatureImage.Length == 0)
            return BadRequest("The signature image is required.");

        await using var certificateStream = new MemoryStream();
        await file.CopyToAsync(certificateStream, cancellationToken);

        await using var signatureStream = new MemoryStream();
        await signatureImage.CopyToAsync(signatureStream, cancellationToken);

        var result = await _sender.Send(
            new CreateCertificateCommand(
                userId,
                name,
                password,
                certificateStream.ToArray(),
                file.FileName,
                signatureStream.ToArray(),
                signatureImage.FileName),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteCertificateCommand(id), cancellationToken);
        return result.ToActionResult();
    }
}
