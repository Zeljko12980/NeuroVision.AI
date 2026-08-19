using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitution.Command.Create
{
    public sealed record CreateHealthInstitutionCommand(CreateHealthInstitutionRequest Request) : ICommand<Result<HealthInstitutionResponse>>;

public sealed class CreateHealthInstitutionCommandValidator : AbstractValidator<CreateHealthInstitutionCommand>
{
    public CreateHealthInstitutionCommandValidator()
    {
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Request.TypeCode).NotEmpty();
        RuleFor(x => x.Request.CountryCode).NotEmpty().MaximumLength(3);
    }
}
}
