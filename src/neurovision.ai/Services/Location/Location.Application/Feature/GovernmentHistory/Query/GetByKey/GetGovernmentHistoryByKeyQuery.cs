using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentHistory.Query.GetByKey
{
    public sealed record GetGovernmentHistoryByKeyQuery(string CountryCode, int SequenceNumber) : IQuery<Result<GovernmentHistoryResponse>>;
}
