using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionType.Command.Create
{
    public sealed record CreateRegionTypeCommand(CreateRegionTypeRequest Request) : ICommand<Result<RegionTypeResponse>>;

public sealed class CreateRegionTypeCommandValidator : AbstractValidator<CreateRegionTypeCommand>
{
    public CreateRegionTypeCommandValidator()
    {
        RuleFor(x => x.Request.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(120);
    }
}
}
