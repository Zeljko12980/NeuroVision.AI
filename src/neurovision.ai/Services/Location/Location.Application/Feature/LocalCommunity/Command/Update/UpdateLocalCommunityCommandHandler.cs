using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunity.Command.Update
{
    public sealed class UpdateLocalCommunityCommandHandler : ICommandHandler<UpdateLocalCommunityCommand, Result<LocalCommunityResponse>>
    {
        private readonly ILocalCommunityService _service;

        public UpdateLocalCommunityCommandHandler(ILocalCommunityService service)
        {
            _service = service;
        }

        public async Task<Result<LocalCommunityResponse>> Handle(UpdateLocalCommunityCommand command, CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(command.CountryCode, command.MunicipalityCode, command.Identifier, command.Request, cancellationToken);
        }
    }
}
