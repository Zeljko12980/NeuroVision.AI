using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.Municipality.Command.Delete
{
    public sealed class DeleteMunicipalityCommandHandler : ICommandHandler<DeleteMunicipalityCommand, Result<bool>>
    {
        private readonly IMunicipalityService _service;

        public DeleteMunicipalityCommandHandler(IMunicipalityService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteMunicipalityCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.CountryCode, command.Code, cancellationToken);
        }
    }
}
