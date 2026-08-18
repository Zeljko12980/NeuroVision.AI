namespace MailService.Application.EventHandlers;

public class SetPasswordEventHandler : IConsumer<SetPasswordEvent>
{
    private readonly ISender _sender;
    private readonly ILogger<SetPasswordEventHandler> _logger;

    public SetPasswordEventHandler(ISender sender, ILogger<SetPasswordEventHandler> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SetPasswordEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("SetPasswordEvent received. Email={Email}", message.Email);

        var result = await _sender.Send(
            new SendTemplatedEmailCommand(
                message.Email,
                EmailTemplateCodes.SetPassword,
                new Dictionary<string, string>
                {
                    [EmailPlaceholderKeys.Email] = message.Email,
                    [EmailPlaceholderKeys.SetPasswordUrl] = message.Url
                }),
            context.CancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("SetPasswordEvent failed. Email={Email}, Error={Error}", message.Email, result.Error);
            throw new InvalidOperationException(result.Error);
        }
    }
}
