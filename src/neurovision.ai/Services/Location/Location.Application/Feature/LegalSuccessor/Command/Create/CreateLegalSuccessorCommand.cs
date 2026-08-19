using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LegalSuccessor.Command.Create
{
    public sealed record CreateLegalSuccessorCommand(CreateLegalSuccessorRequest Request) : ICommand<Result<LegalSuccessorResponse>>;

public sealed class CreateLegalSuccessorCommandValidator : AbstractValidator<CreateLegalSuccessorCommand>
{
    public CreateLegalSuccessorCommandValidator()
    {
        RuleFor(x => x.Request.SuccessorCountryCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.Request.PredecessorCountryCode).NotEmpty().MaximumLength(3);
    }
}
}
