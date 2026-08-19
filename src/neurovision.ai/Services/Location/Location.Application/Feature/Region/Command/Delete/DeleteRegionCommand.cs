using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.Region.Command.Delete
{
    public sealed record DeleteRegionCommand(string TypeCode, short Code) : ICommand<Result<bool>>;

public sealed class DeleteRegionCommandValidator : AbstractValidator<DeleteRegionCommand>
{
    public DeleteRegionCommandValidator()
    {
        RuleFor(x => x.TypeCode).NotEmpty();
    }
}
}
