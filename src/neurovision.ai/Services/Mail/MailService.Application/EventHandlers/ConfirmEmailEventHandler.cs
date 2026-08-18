namespace MailService.Application.EventHandlers;

public class ConfirmEmailEventHandler : IConsumer<ConfirmEmailEvent>
{
    private readonly ISender _sender;
    private readonly ILogger<ConfirmEmailEventHandler> _logger;

    public ConfirmEmailEventHandler(ISender sender, ILogger<ConfirmEmailEventHandler> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ConfirmEmailEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "ConfirmEmailEvent received. UserId={UserId}, Email={Email}",
            message.UserId,
            message.Email);

        var result = await _sender.Send(
            new SendTemplatedEmailCommand(
                message.Email,
                EmailTemplateCodes.EmailConfirmation,
                new Dictionary<string, string>
                {
                    [EmailPlaceholderKeys.FullName] = message.Email,
                    [EmailPlaceholderKeys.ConfirmationUrl] = message.token
                }),
            context.CancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError(
                "ConfirmEmailEvent failed. UserId={UserId}, Error={Error}",
                message.UserId,
                result.Error);
            throw new InvalidOperationException(result.Error);
        }
    }
}
