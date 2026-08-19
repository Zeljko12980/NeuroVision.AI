namespace PdfService.Application.Commands.Templates;

public sealed record UpdatePdfTemplateCommand(Guid Id, UpdatePdfTemplateRequest Request)
    : ICommand<Result<PdfTemplateResponse>>;

public sealed class UpdatePdfTemplateCommandValidator : AbstractValidator<UpdatePdfTemplateCommand>
{
    public UpdatePdfTemplateCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Request.HtmlContent)
            .NotEmpty();

        RuleFor(x => x.Request.Version)
            .GreaterThan(0);
    }
}

public sealed class UpdatePdfTemplateCommandHandler
    : ICommandHandler<UpdatePdfTemplateCommand, Result<PdfTemplateResponse>>
{
    private readonly IRepository<PdfTemplate, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePdfTemplateCommandHandler(
        IRepository<PdfTemplate, Guid> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PdfTemplateResponse>> Handle(
        UpdatePdfTemplateCommand command,
        CancellationToken cancellationToken)
    {
        var template = await _repository.GetByIdAsync(command.Id, cancellationToken);

        if (template is null)
        {
            return Result<PdfTemplateResponse>.Fail(
                "PDF template not found.",
                HttpStatusCode.NotFound);
        }

        template.Update(
            command.Request.Name,
            command.Request.HtmlContent,
            command.Request.Version,
            command.Request.IsActive);

        _repository.Update(template);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PdfTemplateResponse>.Ok(template.ToResponse());
    }
}
