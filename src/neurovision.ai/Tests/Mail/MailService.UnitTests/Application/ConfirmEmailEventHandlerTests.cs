using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Results;
using MailService.Application.Commands;
using MailService.Application.EventHandlers;
using MailService.Domain.Constants;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;

namespace MailService.UnitTests.Application;

public class ConfirmEmailEventHandlerTests
{
    private readonly ISender _sender = Substitute.For<ISender>();
    private readonly ConfirmEmailEventHandler _handler;

    public ConfirmEmailEventHandlerTests()
    {
        _handler = new ConfirmEmailEventHandler(_sender, NullLogger<ConfirmEmailEventHandler>.Instance);
    }

    [Fact]
    public async Task Consume_MapsEventToConfirmTemplate()
    {
        var userId = Guid.NewGuid();
        _sender.Send(Arg.Any<SendTemplatedEmailCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        var context = Substitute.For<ConsumeContext<ConfirmEmailEvent>>();
        context.Message.Returns(new ConfirmEmailEvent(userId, "jane@neurovision.ai", "https://app/confirm"));
        context.CancellationToken.Returns(CancellationToken.None);

        await _handler.Consume(context);

        await _sender.Received(1).Send(
            Arg.Is<SendTemplatedEmailCommand>(command =>
                command.To == "jane@neurovision.ai"
                && command.TemplateId == EmailTemplateCodes.EmailConfirmation
                && command.Placeholders[EmailPlaceholderKeys.ConfirmationUrl] == "https://app/confirm"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WhenSendFails_Throws()
    {
        _sender.Send(Arg.Any<SendTemplatedEmailCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail("smtp down"));

        var context = Substitute.For<ConsumeContext<ConfirmEmailEvent>>();
        context.Message.Returns(new ConfirmEmailEvent(Guid.NewGuid(), "jane@neurovision.ai", "https://app/confirm"));
        context.CancellationToken.Returns(CancellationToken.None);

        var act = () => _handler.Consume(context);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
