using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.GovernmentHistory.Command.Delete
{
    public sealed record DeleteGovernmentHistoryCommand(string CountryCode, int SequenceNumber) : ICommand<Result<bool>>;

public sealed class DeleteGovernmentHistoryCommandValidator : AbstractValidator<DeleteGovernmentHistoryCommand>
{
    public DeleteGovernmentHistoryCommandValidator()
    {
        RuleFor(x => x.CountryCode).NotEmpty();
    }
}
}
