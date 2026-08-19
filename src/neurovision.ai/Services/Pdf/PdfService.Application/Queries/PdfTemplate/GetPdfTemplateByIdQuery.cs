namespace PdfService.Application.Queries.Templates;

public sealed record GetPdfTemplateByIdQuery(Guid Id) : IQuery<Result<PdfTemplateResponse>>;

public sealed class GetPdfTemplateByIdQueryHandler
    : IQueryHandler<GetPdfTemplateByIdQuery, Result<PdfTemplateResponse>>
{
    private readonly IRepository<PdfTemplate, Guid> _repository;
    private readonly IPdfTemplateReadStore _readStore;

    public GetPdfTemplateByIdQueryHandler(
        IRepository<PdfTemplate, Guid> repository,
        IPdfTemplateReadStore readStore)
    {
        _repository = repository;
        _readStore = readStore;
    }

    public async Task<Result<PdfTemplateResponse>> Handle(
        GetPdfTemplateByIdQuery query,
        CancellationToken cancellationToken)
    {
        var template = await _repository.GetByIdAsync(query.Id, cancellationToken);
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
