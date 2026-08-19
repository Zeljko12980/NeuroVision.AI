using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Municipality.Command.Create
{
    public sealed class CreateMunicipalityCommandHandler : ICommandHandler<CreateMunicipalityCommand, Result<MunicipalityResponse>>
    {
        private readonly IMunicipalityService _service;

        public CreateMunicipalityCommandHandler(IMunicipalityService service)
        {
            _service = service;
        }

        public async Task<Result<MunicipalityResponse>> Handle(CreateMunicipalityCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
