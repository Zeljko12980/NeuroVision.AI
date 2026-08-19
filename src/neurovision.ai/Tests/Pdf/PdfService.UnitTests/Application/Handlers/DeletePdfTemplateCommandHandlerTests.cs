using BuildingBlocks.Persistence;
using PdfService.Application.Commands.Templates;
using System.Net;

namespace PdfService.UnitTests.Application.Handlers;

public class DeletePdfTemplateCommandHandlerTests
{
    private readonly IRepository<PdfTemplate, Guid> _repository = Substitute.For<IRepository<PdfTemplate, Guid>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeletePdfTemplateCommandHandler _handler;

    public DeletePdfTemplateCommandHandlerTests()
    {
        _handler = new DeletePdfTemplateCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenTemplateMissing_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((PdfTemplate?)null);

        var result = await _handler.Handle(new DeletePdfTemplateCommand(id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _repository.DidNotReceive().Delete(Arg.Any<PdfTemplate>());
    }

    [Fact]
    public async Task Handle_WhenTemplateExists_DeletesTemplate()
    {
        var template = PdfTemplate.Create("MAIL", "Mail", "<p>hi</p>");
        _repository.GetByIdAsync(template.Id, Arg.Any<CancellationToken>()).Returns(template);

        var result = await _handler.Handle(new DeletePdfTemplateCommand(template.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _repository.Received(1).Delete(template);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
