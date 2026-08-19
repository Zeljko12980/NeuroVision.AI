using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.RegionComposition.Command.Delete
{
    public sealed record DeleteRegionCompositionCommand(string ParentRegionTypeCode, short ParentRegionCode, string MemberRegionTypeCode, short MemberRegionCode) : ICommand<Result<bool>>;

public sealed class DeleteRegionCompositionCommandValidator : AbstractValidator<DeleteRegionCompositionCommand>
{
    public DeleteRegionCompositionCommandValidator()
    {
        RuleFor(x => x.ParentRegionTypeCode).NotEmpty();
        RuleFor(x => x.MemberRegionTypeCode).NotEmpty();
    }
}
}
