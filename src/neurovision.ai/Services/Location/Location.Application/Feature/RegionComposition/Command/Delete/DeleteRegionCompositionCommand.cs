using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.RegionComposition.Command.Delete
{
    public sealed record DeleteRegionCompositionCommand(string ParentRegionTypeCode, short ParentRegionCode, string MemberRegionTypeCode, short MemberRegionCode) : ICommand<Result<bool>>;
}
