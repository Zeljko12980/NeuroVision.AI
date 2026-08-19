using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionComposition.Command.Create
{
    public sealed record CreateRegionCompositionCommand(CreateRegionCompositionRequest Request) : ICommand<Result<RegionCompositionResponse>>;

public sealed class CreateRegionCompositionCommandValidator : AbstractValidator<CreateRegionCompositionCommand>
{
    public CreateRegionCompositionCommandValidator()
    {
        RuleFor(x => x.Request.ParentRegionTypeCode).NotEmpty();
        RuleFor(x => x.Request.MemberRegionTypeCode).NotEmpty();
    }
}
}
