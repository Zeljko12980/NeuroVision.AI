using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.MunicipalitySettlementCoverage.Command.Create
{
    public sealed record CreateMunicipalitySettlementCoverageCommand(CreateMunicipalitySettlementCoverageRequest Request) : ICommand<Result<MunicipalitySettlementCoverageResponse>>;

public sealed class CreateMunicipalitySettlementCoverageCommandValidator : AbstractValidator<CreateMunicipalitySettlementCoverageCommand>
{
    public CreateMunicipalitySettlementCoverageCommandValidator()
    {
        RuleFor(x => x.Request.CountryCode).NotEmpty().MaximumLength(3);
    }
}
}
