using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Settlement.Command.Create
{
    public sealed record CreateSettlementCommand(CreateSettlementRequest Request) : ICommand<Result<SettlementResponse>>;

public sealed class CreateSettlementCommandValidator : AbstractValidator<CreateSettlementCommand>
{
    public CreateSettlementCommandValidator()
    {
        RuleFor(x => x.Request.CountryCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(120);
    }
}
}
