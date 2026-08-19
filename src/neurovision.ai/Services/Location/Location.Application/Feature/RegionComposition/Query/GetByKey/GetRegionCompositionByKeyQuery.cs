using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionComposition.Query.GetByKey
{
    public sealed record GetRegionCompositionByKeyQuery(string ParentRegionTypeCode, short ParentRegionCode, string MemberRegionTypeCode, short MemberRegionCode) : IQuery<Result<RegionCompositionResponse>>;
}
