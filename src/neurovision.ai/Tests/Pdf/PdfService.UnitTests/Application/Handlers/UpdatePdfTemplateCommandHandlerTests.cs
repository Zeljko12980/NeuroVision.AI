using BuildingBlocks.Persistence;
using PdfService.Application.Commands.Templates;
using PdfService.Application.Common.Requests;
using System.Net;

namespace PdfService.UnitTests.Application.Handlers;

public class UpdatePdfTemplateCommandHandlerTests
{
    private readonly IRepository<PdfTemplate, Guid> _repository = Substitute.For<IRepository<PdfTemplate, Guid>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdatePdfTemplateCommandHandler _handler;

    public UpdatePdfTemplateCommandHandlerTests()
    {
        _handler = new UpdatePdfTemplateCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenTemplateMissing_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((PdfTemplate?)null);

        var result = await _handler.Handle(Command(id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _repository.DidNotReceive().Update(Arg.Any<PdfTemplate>());
    }

    [Fact]
    public async Task Handle_WhenTemplateExists_UpdatesAndReturnsResponse()
    {
        var template = PdfTemplate.Create("MAIL", "Old", "<p>old</p>");
        _repository.GetByIdAsync(template.Id, Arg.Any<CancellationToken>()).Returns(template);

        var result = await _handler.Handle(Command(template.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("New");
        result.Value.HtmlContent.Should().Be("<p>new</p>");
        result.Value.Version.Should().Be(2);
        result.Value.IsActive.Should().BeFalse();
        _repository.Received(1).Update(template);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static UpdatePdfTemplateCommand Command(Guid id) =>
        new(id, new UpdatePdfTemplateRequest
        {
            Name = "New",
            HtmlContent = "<p>new</p>",
            Version = 2,
            IsActive = false
        });
}
