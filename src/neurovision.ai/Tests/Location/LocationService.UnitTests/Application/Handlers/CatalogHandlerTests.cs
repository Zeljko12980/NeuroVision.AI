using BuildingBlocks.Persistence;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;
using LocationService.Application.Feature.Capital.Command.Create;
using LocationService.Application.Feature.Capital.Command.Delete;
using LocationService.Application.Feature.GovernmentType.Command.Create;
using System.Net;

namespace LocationService.UnitTests.Application.Handlers;

public class CatalogHandlerTests
{
    [Fact]
    public async Task CreateGovernmentType_WhenDuplicate_ReturnsConflict()
    {
        var reads = Substitute.For<ILocationReadStore<GovernmentTypeResponse>>();
        var writes = Substitute.For<ILocationWriteStore>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        reads.ExistsAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(true);

        var handler = new CreateGovernmentTypeCommandHandler(reads, writes, unitOfWork);

        var result = await handler.Handle(
            new CreateGovernmentTypeCommand(new CreateGovernmentTypeRequest
            {
                Code = "REP",
                Name = "Republic"
            }),
            CancellationToken.None);

        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        await writes.DidNotReceive().AddAsync(Arg.Any<GovernmentType>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCapital_PersistsPeriod()
    {
        var reads = Substitute.For<ILocationReadStore<CapitalResponse>>();
        var writes = Substitute.For<ILocationWriteStore>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        reads.ExistsAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(false);

        var handler = new CreateCapitalCommandHandler(reads, writes, unitOfWork);

        var result = await handler.Handle(
            new CreateCapitalCommand(new CreateCapitalRequest
            {
                CountryCode = "ba",
                SettlementCode = 1,
                SequenceNumber = 1,
                From = new DateTime(1995, 12, 14)
            }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.CountryCode.Should().Be("BA");
        await writes.Received(1).AddAsync(Arg.Any<Capital>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCapital_WhenMissing_ReturnsNotFound()
    {
        var writes = Substitute.For<ILocationWriteStore>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        writes.FindAsync<Capital>(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns((Capital?)null);

        var handler = new DeleteCapitalCommandHandler(writes, unitOfWork);

        var result = await handler.Handle(
            new DeleteCapitalCommand("BA", 1, 1),
            CancellationToken.None);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
