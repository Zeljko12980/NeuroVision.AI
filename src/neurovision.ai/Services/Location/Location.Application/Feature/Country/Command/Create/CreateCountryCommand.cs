using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;


namespace LocationService.Application.Feature.Country.Command.Create
{
    public sealed record CreateCountryCommand(CreateCountryRequest Request) : ICommand<Result<CountryResponse>>;

}
