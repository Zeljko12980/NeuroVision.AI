using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionType.Query.GetByKey
{
    public sealed record GetRegionTypeByKeyQuery(string Code) : IQuery<Result<RegionTypeResponse>>;
}
