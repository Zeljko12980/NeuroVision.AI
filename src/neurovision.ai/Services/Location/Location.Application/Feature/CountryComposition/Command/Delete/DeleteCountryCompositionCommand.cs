using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.CountryComposition.Command.Delete
{
    public sealed record DeleteCountryCompositionCommand(string UnionCountryCode, string MemberCountryCode, int SequenceNumber) : ICommand<Result<bool>>;

public sealed class DeleteCountryCompositionCommandValidator : AbstractValidator<DeleteCountryCompositionCommand>
{
    public DeleteCountryCompositionCommandValidator()
    {
        RuleFor(x => x.UnionCountryCode).NotEmpty();
    }
}
}
