using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Capital.Command.Update
{
    public sealed record UpdateCapitalCommand(UpdateCapitalRequest Request, string CountryCode, int SettlementCode, int SequenceNumber) : ICommand<Result<CapitalResponse>>;
}
