using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.RegionSettlementCoverage.Command.Delete
{
    public sealed record DeleteRegionSettlementCoverageCommand(string RegionTypeCode, short RegionCode, string CountryCode, int SettlementCode) : ICommand<Result<bool>>;

public sealed class DeleteRegionSettlementCoverageCommandValidator : AbstractValidator<DeleteRegionSettlementCoverageCommand>
{
    public DeleteRegionSettlementCoverageCommandValidator()
    {
        RuleFor(x => x.RegionTypeCode).NotEmpty();
        RuleFor(x => x.CountryCode).NotEmpty();
    }
}
}
