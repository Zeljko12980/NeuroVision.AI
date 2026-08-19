using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.HealthInstitutionType.Command.Delete
{
    public sealed record DeleteHealthInstitutionTypeCommand(string Code) : ICommand<Result<bool>>;

public sealed class DeleteHealthInstitutionTypeCommandValidator : AbstractValidator<DeleteHealthInstitutionTypeCommand>
{
    public DeleteHealthInstitutionTypeCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
    }
}
}
