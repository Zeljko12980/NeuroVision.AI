using BuildingBlocks.Pagination;
using BuildingBlocks.Persistence;
using PdfService.Application.Common.Interfaces;
using PdfService.Application.Common.Requests;
using PdfService.Application.Queries.Templates;
using System.Net;

namespace PdfService.UnitTests.Application.Handlers;

public class GetPdfTemplateByIdQueryHandlerTests
{
    private readonly IRepository<PdfTemplate, Guid> _repository = Substitute.For<IRepository<PdfTemplate, Guid>>();
    private readonly IPdfTemplateReadStore _readStore = Substitute.For<IPdfTemplateReadStore>();
    private readonly GetPdfTemplateByIdQueryHandler _handler;

    public GetPdfTemplateByIdQueryHandlerTests()
    {
        _handler = new GetPdfTemplateByIdQueryHandler(_repository, _readStore);
    }

    [Fact]
    public async Task Handle_WhenMissing_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((PdfTemplate?)null);

        var result = await _handler.Handle(new GetPdfTemplateByIdQuery(id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _readStore.DidNotReceive().LoadFieldsAsync(Arg.Any<PdfTemplate>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenFound_LoadsFieldsAndMapsResponse()
    {
        var template = PdfTemplate.Create("MAIL", "Mail", "<p>{{Name}}</p>");
        _repository.GetByIdAsync(template.Id, Arg.Any<CancellationToken>()).Returns(template);

        var result = await _handler.Handle(new GetPdfTemplateByIdQuery(template.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("MAIL");
        await _readStore.Received(1).LoadFieldsAsync(template, Arg.Any<CancellationToken>());
    }
}

public class GetPdfTemplateByCodeQueryHandlerTests
{
    private readonly IPdfTemplateReadStore _readStore = Substitute.For<IPdfTemplateReadStore>();
    private readonly GetPdfTemplateByCodeQueryHandler _handler;

    public GetPdfTemplateByCodeQueryHandlerTests()
    {
        _handler = new GetPdfTemplateByCodeQueryHandler(_readStore);
    }

    [Fact]
    public async Task Handle_WhenMissing_ReturnsNotFound()
    {
        _readStore.GetByCodeAsync("MISSING", Arg.Any<CancellationToken>()).Returns((PdfTemplate?)null);

        var result = await _handler.Handle(new GetPdfTemplateByCodeQuery("MISSING"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Handle_WhenFound_LoadsFieldsAndMapsResponse()
    {
        var template = PdfTemplate.Create("MAIL", "Mail", "<p>hi</p>");
        _readStore.GetByCodeAsync("MAIL", Arg.Any<CancellationToken>()).Returns(template);

        var result = await _handler.Handle(new GetPdfTemplateByCodeQuery("MAIL"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Mail");
        await _readStore.Received(1).LoadFieldsAsync(template, Arg.Any<CancellationToken>());
    }
}

public class GetAllPdfTemplatesQueryHandlerTests
{
    private readonly IPdfTemplateReadStore _readStore = Substitute.For<IPdfTemplateReadStore>();
    private readonly GetAllPdfTemplatesQueryHandler _handler;

    public GetAllPdfTemplatesQueryHandlerTests()
    {
        _handler = new GetAllPdfTemplatesQueryHandler(_readStore);
    }

    [Fact]
    public async Task Handle_ReturnsPagedTemplates()
    {
        var template = PdfTemplate.Create("MAIL", "Mail", "<p>hi</p>");
        _readStore.GetPagedAsync("MAIL", 0, 10, Arg.Any<CancellationToken>())
            .Returns((new List<PdfTemplate> { template }, 1));

        var result = await _handler.Handle(
            new GetAllPdfTemplatesQuery(new GetPdfTemplatesRequest("MAIL")),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Count.Should().Be(1);
        result.Value.Data.Should().ContainSingle(item => item.Code == "MAIL");
    }

    [Fact]
    public async Task Handle_WhenPageIndexNegative_UsesZero()
    {
        _readStore.GetPagedAsync(null, 0, 10, Arg.Any<CancellationToken>())
            .Returns((new List<PdfTemplate>(), 0));

        var result = await _handler.Handle(
            new GetAllPdfTemplatesQuery(new GetPdfTemplatesRequest(null) { PageIndex = -3, PageSize = 10 }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.PageIndex.Should().Be(0);
        await _readStore.Received(1).GetPagedAsync(null, 0, 10, Arg.Any<CancellationToken>());
    }
}

public class GetActivePdfTemplatesQueryHandlerTests
{
    private readonly IPdfTemplateReadStore _readStore = Substitute.For<IPdfTemplateReadStore>();
    private readonly GetActivePdfTemplatesQueryHandler _handler;

    public GetActivePdfTemplatesQueryHandlerTests()
    {
        _handler = new GetActivePdfTemplatesQueryHandler(_readStore);
    }

    [Fact]
    public async Task Handle_ReturnsActiveTemplates()
    {
        var template = PdfTemplate.Create("MAIL", "Mail", "<p>hi</p>");
        _readStore.GetActiveAsync(0, 10, Arg.Any<CancellationToken>())
            .Returns((new List<PdfTemplate> { template }, 1));

        var result = await _handler.Handle(
            new GetActivePdfTemplatesQuery(new PaginationRequest(0, 10)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Data.Should().ContainSingle(item => item.IsActive);
    }
}
