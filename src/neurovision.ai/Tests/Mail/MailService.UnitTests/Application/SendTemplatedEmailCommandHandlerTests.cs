using BuildingBlocks.Results;
using MailService.Application.Commands;
using MailService.Application.Common.Interfaces;
using MailService.Domain.Constants;
using MailService.Domain.ValueObjects;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;

namespace MailService.UnitTests.Application;

public class SendTemplatedEmailCommandHandlerTests
{
    private readonly IDocumentGenerator _documentGenerator = Substitute.For<IDocumentGenerator>();
    private readonly IEmailSender _emailSender = Substitute.For<IEmailSender>();
    private readonly SendTemplatedEmailCommandHandler _handler;

    public SendTemplatedEmailCommandHandlerTests()
    {
        _handler = new SendTemplatedEmailCommandHandler(
            _documentGenerator,
            _emailSender,
            NullLogger<SendTemplatedEmailCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenDocumentGenerationFails_ReturnsFailure()
    {
        _documentGenerator.GenerateAsync(
                Arg.Any<string>(),
                Arg.Any<IReadOnlyDictionary<string, string>>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<byte[]>.Fail("pdf down", HttpStatusCode.BadGateway));

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("pdf down");
        await _emailSender.DidNotReceive().SendAsync(Arg.Any<EmailMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSendSucceeds_SendsPdfAttachment()
    {
        _documentGenerator.GenerateAsync(
                EmailTemplateCodes.EmailConfirmation,
                Arg.Any<IReadOnlyDictionary<string, string>>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<byte[]>.Ok(new byte[] { 1, 2, 3 }));
        _emailSender.SendAsync(Arg.Any<EmailMessage>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _emailSender.Received(1).SendAsync(
            Arg.Is<EmailMessage>(message =>
                message.To.Value == "jane@neurovision.ai"
                && message.Subject == "Email Confirmation"
                && message.Attachments.Count == 1
                && message.Attachments[0].FileName == "EmailConfirmation.pdf"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPlaceholderMissing_ReturnsFailure()
    {
        var command = new SendTemplatedEmailCommand(
            "jane@neurovision.ai",
            EmailTemplateCodes.EmailConfirmation,
            new Dictionary<string, string>
            {
                [EmailPlaceholderKeys.FullName] = "Jane"
            });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        await _documentGenerator.DidNotReceive().GenerateAsync(
            Arg.Any<string>(),
            Arg.Any<IReadOnlyDictionary<string, string>>(),
            Arg.Any<CancellationToken>());
    }

    private static SendTemplatedEmailCommand ValidCommand()
        => new(
            "jane@neurovision.ai",
            EmailTemplateCodes.EmailConfirmation,
            new Dictionary<string, string>
            {
                [EmailPlaceholderKeys.FullName] = "Jane",
                [EmailPlaceholderKeys.ConfirmationUrl] = "https://app.neurovision.ai/confirm"
            });
}
