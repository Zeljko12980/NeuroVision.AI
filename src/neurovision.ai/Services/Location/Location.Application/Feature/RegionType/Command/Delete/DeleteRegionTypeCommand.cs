using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.RegionType.Command.Delete
{
    public sealed record DeleteRegionTypeCommand(string Code) : ICommand<Result<bool>>;

public sealed class DeleteRegionTypeCommandValidator : AbstractValidator<DeleteRegionTypeCommand>
{
    public DeleteRegionTypeCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
    }
}
}
