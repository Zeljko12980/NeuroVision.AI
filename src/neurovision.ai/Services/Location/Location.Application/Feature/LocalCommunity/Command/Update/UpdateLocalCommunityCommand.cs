using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunity.Command.Update
{
    public sealed record UpdateLocalCommunityCommand(UpdateLocalCommunityRequest Request, string CountryCode, int MunicipalityCode, int Identifier) : ICommand<Result<LocalCommunityResponse>>;

public sealed class UpdateLocalCommunityCommandValidator : AbstractValidator<UpdateLocalCommunityCommand>
{
    public UpdateLocalCommunityCommandValidator()
    {
        RuleFor(x => x.CountryCode).NotEmpty();
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(120);
    }
}
}
