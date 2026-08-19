using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunity.Command.Create
{
    public sealed record CreateLocalCommunityCommand(CreateLocalCommunityRequest Request) : ICommand<Result<LocalCommunityResponse>>;

public sealed class CreateLocalCommunityCommandValidator : AbstractValidator<CreateLocalCommunityCommand>
{
    public CreateLocalCommunityCommandValidator()
    {
        RuleFor(x => x.Request.CountryCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(120);
    }
}
}
