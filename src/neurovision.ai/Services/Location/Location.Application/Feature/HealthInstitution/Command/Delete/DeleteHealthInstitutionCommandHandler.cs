using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.HealthInstitution.Command.Delete
{
    public sealed class DeleteHealthInstitutionCommandHandler : ICommandHandler<DeleteHealthInstitutionCommand, Result<bool>>
    {
        private readonly IHealthInstitutionService _service;

        public DeleteHealthInstitutionCommandHandler(IHealthInstitutionService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteHealthInstitutionCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.Id, cancellationToken);
        }
    }
}
