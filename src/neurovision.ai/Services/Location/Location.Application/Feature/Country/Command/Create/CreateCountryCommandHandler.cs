
using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Country.Command.Create
{
    public sealed class CreateCountryCommandHandler : ICommandHandler<CreateCountryCommand, Result<CountryResponse>>
    {
        private readonly ICountryService _countryService;

        public CreateCountryCommandHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }
        public async Task<Result<CountryResponse>> Handle(CreateCountryCommand command, CancellationToken cancellationToken)
        {
            return await _countryService.AddAsync(command.Request, cancellationToken);
        }
    }
}
