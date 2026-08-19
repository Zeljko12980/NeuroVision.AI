using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitutionType.Command.Create
{
    public sealed record CreateHealthInstitutionTypeCommand(CreateHealthInstitutionTypeRequest Request) : ICommand<Result<HealthInstitutionTypeResponse>>;

public sealed class CreateHealthInstitutionTypeCommandValidator : AbstractValidator<CreateHealthInstitutionTypeCommand>
{
    public CreateHealthInstitutionTypeCommandValidator()
    {
        RuleFor(x => x.Request.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(120);
    }
}
}
