using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.LocalCommunity.Command.Delete
{
    public sealed class DeleteLocalCommunityCommandHandler : ICommandHandler<DeleteLocalCommunityCommand, Result<bool>>
    {
        private readonly ILocalCommunityService _service;

        public DeleteLocalCommunityCommandHandler(ILocalCommunityService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteLocalCommunityCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.CountryCode, command.MunicipalityCode, command.Identifier, cancellationToken);
        }
    }
}
