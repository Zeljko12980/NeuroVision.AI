using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.Municipality.Command.Delete
{
    public sealed record DeleteMunicipalityCommand(string CountryCode, int Code) : ICommand<Result<bool>>;
}
