namespace MailService.Application.EventHandlers;

public class TwoFactorCodeGeneratedEventHandler : IConsumer<TwoFactorCodeGeneratedEvent>
{
    private readonly ISender _sender;
    private readonly ILogger<TwoFactorCodeGeneratedEventHandler> _logger;

    public TwoFactorCodeGeneratedEventHandler(
        ISender sender,
        ILogger<TwoFactorCodeGeneratedEventHandler> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TwoFactorCodeGeneratedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("TwoFactorCodeGeneratedEvent received. Email={Email}", message.Email);

        var result = await _sender.Send(
            new SendTemplatedEmailCommand(
                message.Email,
                EmailTemplateCodes.TwoFactor,
                new Dictionary<string, string>
                {
                    [EmailPlaceholderKeys.FullName] = message.FullName,
                    [EmailPlaceholderKeys.Code] = message.Code
                }),
            context.CancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError(
                "TwoFactorCodeGeneratedEvent failed. Email={Email}, Error={Error}",
                message.Email,
                result.Error);
            throw new InvalidOperationException(result.Error);
        }
    }
}
