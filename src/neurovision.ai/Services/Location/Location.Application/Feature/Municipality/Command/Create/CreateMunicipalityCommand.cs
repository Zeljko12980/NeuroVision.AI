using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Municipality.Command.Create
{
    public sealed record CreateMunicipalityCommand(CreateMunicipalityRequest Request) : ICommand<Result<MunicipalityResponse>>;

public sealed class CreateMunicipalityCommandValidator : AbstractValidator<CreateMunicipalityCommand>
{
    public CreateMunicipalityCommandValidator()
    {
        RuleFor(x => x.Request.CountryCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(120);
    }
}
}
