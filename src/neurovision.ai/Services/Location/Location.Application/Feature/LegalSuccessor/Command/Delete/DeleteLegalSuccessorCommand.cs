using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.LegalSuccessor.Command.Delete
{
    public sealed record DeleteLegalSuccessorCommand(string SuccessorCountryCode, string PredecessorCountryCode) : ICommand<Result<bool>>;

public sealed class DeleteLegalSuccessorCommandValidator : AbstractValidator<DeleteLegalSuccessorCommand>
{
    public DeleteLegalSuccessorCommandValidator()
    {
        RuleFor(x => x.SuccessorCountryCode).NotEmpty();
        RuleFor(x => x.PredecessorCountryCode).NotEmpty();
    }
}
}
