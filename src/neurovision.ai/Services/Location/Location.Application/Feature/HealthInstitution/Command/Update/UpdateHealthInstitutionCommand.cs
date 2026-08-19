using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitution.Command.Update
{
    public sealed record UpdateHealthInstitutionCommand(UpdateHealthInstitutionRequest Request, int Id) : ICommand<Result<HealthInstitutionResponse>>;

public sealed class UpdateHealthInstitutionCommandValidator : AbstractValidator<UpdateHealthInstitutionCommand>
{
    public UpdateHealthInstitutionCommandValidator()
    {
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Request.TypeCode).NotEmpty();
        RuleFor(x => x.Request.CountryCode).NotEmpty().MaximumLength(3);
    }
}
}
