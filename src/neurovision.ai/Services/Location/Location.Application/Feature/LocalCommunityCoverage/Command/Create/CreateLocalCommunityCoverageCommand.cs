using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunityCoverage.Command.Create
{
    public sealed record CreateLocalCommunityCoverageCommand(CreateLocalCommunityCoverageRequest Request) : ICommand<Result<LocalCommunityCoverageResponse>>;

public sealed class CreateLocalCommunityCoverageCommandValidator : AbstractValidator<CreateLocalCommunityCoverageCommand>
{
    public CreateLocalCommunityCoverageCommandValidator()
    {
        RuleFor(x => x.Request.CountryCode).NotEmpty().MaximumLength(3);
    }
}
}
