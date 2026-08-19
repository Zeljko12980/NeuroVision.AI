using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.CountryComposition.Command.Create
{
    public sealed class CreateCountryCompositionCommandHandler : ICommandHandler<CreateCountryCompositionCommand, Result<CountryCompositionResponse>>
    {
        private readonly ICountryCompositionService _service;

        public CreateCountryCompositionCommandHandler(ICountryCompositionService service)
        {
            _service = service;
        }

        public async Task<Result<CountryCompositionResponse>> Handle(CreateCountryCompositionCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
