using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionSettlementCoverage.Command.Create
{
    public sealed record CreateRegionSettlementCoverageCommand(CreateRegionSettlementCoverageRequest Request) : ICommand<Result<RegionSettlementCoverageResponse>>;

public sealed class CreateRegionSettlementCoverageCommandValidator : AbstractValidator<CreateRegionSettlementCoverageCommand>
{
    public CreateRegionSettlementCoverageCommandValidator()
    {
        RuleFor(x => x.Request.RegionTypeCode).NotEmpty();
        RuleFor(x => x.Request.CountryCode).NotEmpty().MaximumLength(3);
    }
}
}
