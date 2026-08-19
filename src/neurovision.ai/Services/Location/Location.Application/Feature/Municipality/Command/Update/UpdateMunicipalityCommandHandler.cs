using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Municipality.Command.Update
{
    public sealed class UpdateMunicipalityCommandHandler : ICommandHandler<UpdateMunicipalityCommand, Result<MunicipalityResponse>>
    {
        private readonly IMunicipalityService _service;

        public UpdateMunicipalityCommandHandler(IMunicipalityService service)
        {
            _service = service;
        }

        public async Task<Result<MunicipalityResponse>> Handle(UpdateMunicipalityCommand command, CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(command.CountryCode, command.Code, command.Request, cancellationToken);
        }
    }
}
