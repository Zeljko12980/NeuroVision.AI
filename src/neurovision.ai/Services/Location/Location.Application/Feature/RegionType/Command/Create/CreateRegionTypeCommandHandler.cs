using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionType.Command.Create
{
    public sealed class CreateRegionTypeCommandHandler : ICommandHandler<CreateRegionTypeCommand, Result<RegionTypeResponse>>
    {
        private readonly IRegionTypeService _service;

        public CreateRegionTypeCommandHandler(IRegionTypeService service)
        {
            _service = service;
        }

        public async Task<Result<RegionTypeResponse>> Handle(CreateRegionTypeCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
