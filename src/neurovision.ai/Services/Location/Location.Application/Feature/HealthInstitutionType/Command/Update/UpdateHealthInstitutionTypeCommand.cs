using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitutionType.Command.Update
{
    public sealed record UpdateHealthInstitutionTypeCommand(UpdateHealthInstitutionTypeRequest Request, string Code) : ICommand<Result<HealthInstitutionTypeResponse>>;

public sealed class UpdateHealthInstitutionTypeCommandValidator : AbstractValidator<UpdateHealthInstitutionTypeCommand>
{
    public UpdateHealthInstitutionTypeCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(120);
    }
}
}
