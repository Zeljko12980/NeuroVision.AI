using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.CountryComposition.Command.Create
{
    public sealed record CreateCountryCompositionCommand(CreateCountryCompositionRequest Request) : ICommand<Result<CountryCompositionResponse>>;
}
