namespace PdfService.Application.Commands.Templates;

public sealed record DeletePdfTemplateCommand(Guid Id) : ICommand<Result<bool>>;

public sealed class DeletePdfTemplateCommandHandler
    : ICommandHandler<DeletePdfTemplateCommand, Result<bool>>
{
    private readonly IRepository<PdfTemplate, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePdfTemplateCommandHandler(
        IRepository<PdfTemplate, Guid> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeletePdfTemplateCommand command,
        CancellationToken cancellationToken)
    {
        var template = await _repository.GetByIdAsync(command.Id, cancellationToken);

        if (template is null)
        {
            return Result<bool>.Fail(
                "PDF template not found.",
                HttpStatusCode.NotFound);
        }

        _repository.Delete(template);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
