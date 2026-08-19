using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Settlement.Query.GetByKey
{
    public sealed record GetSettlementByKeyQuery(string CountryCode, int Code) : IQuery<Result<SettlementResponse>>;
}
