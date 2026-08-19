using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitution.Command.Update
{
    public sealed class UpdateHealthInstitutionCommandHandler : ICommandHandler<UpdateHealthInstitutionCommand, Result<HealthInstitutionResponse>>
    {
        private readonly IHealthInstitutionService _service;

        public UpdateHealthInstitutionCommandHandler(IHealthInstitutionService service)
        {
            _service = service;
        }

        public async Task<Result<HealthInstitutionResponse>> Handle(UpdateHealthInstitutionCommand command, CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(command.Id, command.Request, cancellationToken);
        }
    }
}
