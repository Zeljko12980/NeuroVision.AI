using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Municipality.Command.Update
{
    public sealed record UpdateMunicipalityCommand(UpdateMunicipalityRequest Request, string CountryCode, int Code) : ICommand<Result<MunicipalityResponse>>;
}
