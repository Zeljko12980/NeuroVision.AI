using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitution.Command.Create
{
    public sealed class CreateHealthInstitutionCommandHandler : ICommandHandler<CreateHealthInstitutionCommand, Result<HealthInstitutionResponse>>
    {
        private readonly IHealthInstitutionService _service;

        public CreateHealthInstitutionCommandHandler(IHealthInstitutionService service)
        {
            _service = service;
        }

        public async Task<Result<HealthInstitutionResponse>> Handle(CreateHealthInstitutionCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
