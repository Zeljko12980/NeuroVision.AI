using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunity.Command.Create
{
    public sealed class CreateLocalCommunityCommandHandler : ICommandHandler<CreateLocalCommunityCommand, Result<LocalCommunityResponse>>
    {
        private readonly ILocalCommunityService _service;

        public CreateLocalCommunityCommandHandler(ILocalCommunityService service)
        {
            _service = service;
        }

        public async Task<Result<LocalCommunityResponse>> Handle(CreateLocalCommunityCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
