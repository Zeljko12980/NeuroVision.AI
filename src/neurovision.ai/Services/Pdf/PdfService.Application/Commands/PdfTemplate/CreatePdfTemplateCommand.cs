namespace PdfService.Application.Commands.Templates;

public sealed record CreatePdfTemplateCommand(PdfTemplateRequest Request)
    : ICommand<Result<PdfTemplateResponse>>;

public sealed class CreatePdfTemplateCommandValidator : AbstractValidator<CreatePdfTemplateCommand>
{
    public CreatePdfTemplateCommandValidator()
    {
        RuleFor(x => x.Request.Code)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Request.HtmlContent)
            .NotEmpty();

        RuleFor(x => x.Request.Version)
            .GreaterThan(0);
    }
}

public sealed class CreatePdfTemplateCommandHandler
    : ICommandHandler<CreatePdfTemplateCommand, Result<PdfTemplateResponse>>
{
    private readonly IPdfTemplateReadStore _readStore;
    private readonly IRepository<PdfTemplate, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePdfTemplateCommandHandler(
        IPdfTemplateReadStore readStore,
        IRepository<PdfTemplate, Guid> repository,
        IUnitOfWork unitOfWork)
    {
        _readStore = readStore;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PdfTemplateResponse>> Handle(
        CreatePdfTemplateCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var existingId = await _readStore.GetIdByCodeAsync(request.Code, cancellationToken);

        if (existingId.HasValue)
        {
            return Result<PdfTemplateResponse>.Fail(
                "Template code already exists.",
                HttpStatusCode.Conflict);
        }

        var fields = request.Fields.Select(field => PdfTemplateField.Create(
            field.Name,
            field.Type,
            field.Page,
            field.X,
            field.Y,
            field.Width,
            field.Height));

        var template = PdfTemplate.Create(
            request.Code,
            request.Name,
            request.HtmlContent,
            request.Version,
            request.IsActive,
            request.RequiresSignature,
            request.SignaturePage,
            fields: fields);

        await _repository.AddAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PdfTemplateResponse>.Created(template.ToResponse());
    }
}
