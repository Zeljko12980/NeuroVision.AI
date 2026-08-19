using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Settlement.Command.Update
{
    public sealed record UpdateSettlementCommand(UpdateSettlementRequest Request, string CountryCode, int Code) : ICommand<Result<SettlementResponse>>;

public sealed class UpdateSettlementCommandValidator : AbstractValidator<UpdateSettlementCommand>
{
    public UpdateSettlementCommandValidator()
    {
        RuleFor(x => x.CountryCode).NotEmpty();
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(120);
    }
}
}
