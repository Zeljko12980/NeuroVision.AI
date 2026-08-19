using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Capital.Query.GetByKey
{
    public sealed record GetCapitalByKeyQuery(string CountryCode, int SettlementCode, int SequenceNumber) : IQuery<Result<CapitalResponse>>;
}
