using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;


namespace LocationService.Application.Feature.Country.Command.Create
{
    public sealed record CreateCountryCommand(CreateCountryRequest Request) : ICommand<Result<CountryResponse>>;


public sealed class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
{
    public CreateCountryCommandValidator()
    {
        RuleFor(x => x.Request.Code).NotEmpty().MaximumLength(3);
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(120);
    }
}
}
