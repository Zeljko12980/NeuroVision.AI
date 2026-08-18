namespace MailService.Application.EventHandlers;

public class ForgotPasswordEventHandler : IConsumer<ForgotPasswordEvent>
{
    private readonly ISender _sender;
    private readonly ILogger<ForgotPasswordEventHandler> _logger;

    public ForgotPasswordEventHandler(ISender sender, ILogger<ForgotPasswordEventHandler> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ForgotPasswordEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("ForgotPasswordEvent received. Email={Email}", message.Email);

        var result = await _sender.Send(
            new SendTemplatedEmailCommand(
                message.Email,
                EmailTemplateCodes.ForgotPassword,
                new Dictionary<string, string>
                {
                    [EmailPlaceholderKeys.Email] = message.Email,
                    [EmailPlaceholderKeys.SetPasswordUrl] = message.Url
                }),
            context.CancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("ForgotPasswordEvent failed. Email={Email}, Error={Error}", message.Email, result.Error);
            throw new InvalidOperationException(result.Error);
        }
    }
}
