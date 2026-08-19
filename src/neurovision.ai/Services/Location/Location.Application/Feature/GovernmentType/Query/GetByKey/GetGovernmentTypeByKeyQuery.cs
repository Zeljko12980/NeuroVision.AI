using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentType.Query.GetByKey
{
    public sealed record GetGovernmentTypeByKeyQuery(string Code) : IQuery<Result<GovernmentTypeResponse>>;
}
