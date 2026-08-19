
using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Country.Command.Update
{
    public sealed record UpdateCountryCommand(UpdateCountryRequest Request, string Code) : ICommand<Result<CountryResponse>>;

}
