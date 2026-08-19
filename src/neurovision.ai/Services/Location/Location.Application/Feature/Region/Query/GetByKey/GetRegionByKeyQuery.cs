using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Region.Query.GetByKey
{
    public sealed record GetRegionByKeyQuery(string TypeCode, short Code) : IQuery<Result<RegionResponse>>;
}
