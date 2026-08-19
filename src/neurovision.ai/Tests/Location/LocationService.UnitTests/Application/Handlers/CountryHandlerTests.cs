using BuildingBlocks.Persistence;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;
using LocationService.Application.Feature.Country.Command.Create;
using LocationService.Application.Feature.Country.Command.Delete;
using LocationService.Application.Feature.Country.Command.Update;
using LocationService.Application.Feature.Country.Query.GetAll;
using LocationService.Application.Feature.Country.Query.GetByCode;
using System.Net;

namespace LocationService.UnitTests.Application.Handlers;

public class CountryHandlerTests
{
    private readonly ILocationReadStore<CountryResponse> _reads = Substitute.For<ILocationReadStore<CountryResponse>>();
    private readonly ILocationWriteStore _writes = Substitute.For<ILocationWriteStore>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Create_WhenCountryExists_ReturnsConflict()
    {
        _reads.ExistsAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(true);
        var handler = new CreateCountryCommandHandler(_reads, _writes, _unitOfWork);

        var result = await handler.Handle(
            new CreateCountryCommand(new CreateCountryRequest
            {
                Code = "BA",
                Name = "Bosnia",
                FoundingDate = new DateTime(1995, 12, 14)
            }),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        await _writes.DidNotReceive().AddAsync(Arg.Any<Country>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_WhenCountryIsNew_PersistsEntity()
    {
        _reads.ExistsAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(false);
        var handler = new CreateCountryCommandHandler(_reads, _writes, _unitOfWork);

        var result = await handler.Handle(
            new CreateCountryCommand(new CreateCountryRequest
            {
                Code = "ba",
                Name = "Bosnia",
                FoundingDate = new DateTime(1995, 12, 14),
                CallingCode = 387
            }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.Code.Should().Be("BA");
        await _writes.Received(1).AddAsync(Arg.Any<Country>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_WhenMissing_ReturnsNotFound()
    {
        _writes.FindAsync<Country>(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns((Country?)null);
        var handler = new UpdateCountryCommandHandler(_writes, _unitOfWork);

        var result = await handler.Handle(
            new UpdateCountryCommand(new UpdateCountryRequest { Name = "New", FoundingDate = DateTime.UtcNow }, "BA"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_WhenFound_SavesChanges()
    {
        var country = Country.Create("BA", "Old", new DateTime(1995, 1, 1));
        _writes.FindAsync<Country>(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(country);
        var handler = new UpdateCountryCommandHandler(_writes, _unitOfWork);

        var result = await handler.Handle(
            new UpdateCountryCommand(
                new UpdateCountryRequest
                {
                    Name = "Bosnia and Herzegovina",
                    FoundingDate = new DateTime(1995, 12, 14),
                    GovernmentTypeCode = "REP"
                },
                "BA"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Bosnia and Herzegovina");
        _writes.Received(1).Update(country);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_WhenMissing_ReturnsNotFound()
    {
        _writes.FindAsync<Country>(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns((Country?)null);
        var handler = new DeleteCountryCommandHandler(_writes, _unitOfWork);

        var result = await handler.Handle(new DeleteCountryCommand("BA"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _writes.DidNotReceive().Remove(Arg.Any<Country>());
    }

    [Fact]
    public async Task Delete_WhenFound_RemovesEntity()
    {
        var country = Country.Create("BA", "Bosnia", new DateTime(1995, 12, 14));
        _writes.FindAsync<Country>(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(country);
        var handler = new DeleteCountryCommandHandler(_writes, _unitOfWork);

        var result = await handler.Handle(new DeleteCountryCommand("BA"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _writes.Received(1).Remove(country);
    }

    [Fact]
    public async Task GetByCode_WhenMissing_ReturnsNotFound()
    {
        _reads.GetByKeyAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns((CountryResponse?)null);
        var handler = new GetByCodeQueryHandler(_reads);

        var result = await handler.Handle(new GetByCodeQuery("XX"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByCode_WhenFound_ReturnsResponse()
    {
        var response = new CountryResponse { Code = "BA", Name = "Bosnia" };
        _reads.GetByKeyAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(response);
        var handler = new GetByCodeQueryHandler(_reads);

        var result = await handler.Handle(new GetByCodeQuery("BA"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("BA");
    }

    [Fact]
    public async Task GetAll_ReturnsPagedResult()
    {
        _reads.CountAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(1);
        _reads.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(new List<CountryResponse> { new() { Code = "BA", Name = "Bosnia" } });
        var handler = new GetAllCountriesQueryHandler(_reads);

        var result = await handler.Handle(
            new GetAllCountriesQuery(new GetCountriesRequest(null, null, false)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Count.Should().Be(1);
        result.Value.Data.Should().ContainSingle(item => item.Code == "BA");
    }
}
