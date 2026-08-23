using BuildingBlocks.Results;
using LocationService.API.Contracts;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.Country.Command.Create;
using LocationService.Application.Feature.Country.Command.Delete;
using LocationService.Application.Feature.Country.Command.Update;
using LocationService.Application.Feature.Country.Query.GetAll;
using LocationService.Application.Feature.Country.Query.GetByCode;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.Staff)]
public class CountryController : ControllerBase
{
    private readonly ISender _sender;

    public CountryController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetCountriesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllCountriesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetByCode(
        [FromRoute] string code,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetByCodeQuery(code), cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Policy = AuthPolicies.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromForm] CreateCountryForm form,
        CancellationToken cancellationToken)
    {
        var request = new CreateCountryRequest
        {
            Code = form.Code,
            Name = form.Name,
            FoundingDate = form.FoundingDate,
            CapitalSettlementCode = form.CapitalSettlementCode,
            GovernmentTypeCode = form.GovernmentTypeCode,
            CallingCode = form.CallingCode,
            Anthem = await form.Anthem.ToBytesAsync(cancellationToken),
            CoatOfArms = await form.CoatOfArms.ToBytesAsync(cancellationToken),
            Flag = await form.Flag.ToBytesAsync(cancellationToken)
        };

        var result = await _sender.Send(new CreateCountryCommand(request), cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Policy = AuthPolicies.SuperAdmin)]
    [HttpDelete("{code}")]
    public async Task<IActionResult> Delete(
        [FromRoute] string code,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteCountryCommand(code), cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Policy = AuthPolicies.SuperAdmin)]
    [HttpPut("{code}")]
    public async Task<IActionResult> Update(
        [FromRoute] string code,
        [FromForm] UpdateCountryForm form,
        CancellationToken cancellationToken)
    {
        var request = new UpdateCountryRequest
        {
            Name = form.Name,
            FoundingDate = form.FoundingDate,
            CapitalSettlementCode = form.CapitalSettlementCode,
            GovernmentTypeCode = form.GovernmentTypeCode,
            CallingCode = form.CallingCode,
            Anthem = await form.Anthem.ToBytesAsync(cancellationToken),
            CoatOfArms = await form.CoatOfArms.ToBytesAsync(cancellationToken),
            Flag = await form.Flag.ToBytesAsync(cancellationToken)
        };

        var result = await _sender.Send(new UpdateCountryCommand(request, code), cancellationToken);
        return result.ToActionResult();
    }
}
