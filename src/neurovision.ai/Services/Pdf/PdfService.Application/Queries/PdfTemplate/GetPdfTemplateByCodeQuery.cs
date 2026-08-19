namespace PdfService.Application.Queries.Templates;

public sealed record GetPdfTemplateByCodeQuery(string Code) : IQuery<Result<PdfTemplateResponse>>;

public sealed class GetPdfTemplateByCodeQueryHandler
    : IQueryHandler<GetPdfTemplateByCodeQuery, Result<PdfTemplateResponse>>
{
    private readonly IPdfTemplateReadStore _readStore;

    public GetPdfTemplateByCodeQueryHandler(IPdfTemplateReadStore readStore)
    {
        _readStore = readStore;
    }

    public async Task<Result<PdfTemplateResponse>> Handle(
        GetPdfTemplateByCodeQuery query,
        CancellationToken cancellationToken)
    {
        var template = await _readStore.GetByCodeAsync(query.Code, cancellationToken);
        if (template is null)
        {
            return Result<PdfTemplateResponse>.Fail(
                "PDF template not found.",
                HttpStatusCode.NotFound);
        }

        await _readStore.LoadFieldsAsync(template, cancellationToken);
        return Result<PdfTemplateResponse>.Ok(template.ToResponse());
    }
}
