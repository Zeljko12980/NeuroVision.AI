using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentHistory.Command.Update
{
    public sealed record UpdateGovernmentHistoryCommand(UpdateGovernmentHistoryRequest Request, string CountryCode, int SequenceNumber) : ICommand<Result<GovernmentHistoryResponse>>;

public sealed class UpdateGovernmentHistoryCommandValidator : AbstractValidator<UpdateGovernmentHistoryCommand>
{
    public UpdateGovernmentHistoryCommandValidator()
    {
        RuleFor(x => x.CountryCode).NotEmpty();
        RuleFor(x => x.Request.GovernmentTypeCode).NotEmpty();
    }
}
}
