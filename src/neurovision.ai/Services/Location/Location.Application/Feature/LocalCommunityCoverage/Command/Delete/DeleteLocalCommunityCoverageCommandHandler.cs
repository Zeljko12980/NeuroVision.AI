using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.LocalCommunityCoverage.Command.Delete
{
    public sealed class DeleteLocalCommunityCoverageCommandHandler : ICommandHandler<DeleteLocalCommunityCoverageCommand, Result<bool>>
    {
        private readonly ILocalCommunityCoverageService _service;

        public DeleteLocalCommunityCoverageCommandHandler(ILocalCommunityCoverageService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteLocalCommunityCoverageCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.CountryCode, command.MunicipalityCode, command.LocalCommunityIdentifier, command.SettlementCode, cancellationToken);
        }
    }
}
