namespace MailService.API.Handlers
{
    public class SetPasswordEventHandler : IConsumer<SetPasswordEvent>
    {
        private const string SetPasswordTemplateCode = "SetPasswordTemplate";

        private readonly IEmailService _emailService;
        private readonly IPdfServiceClient _pdfServiceClient;

        public SetPasswordEventHandler(
            IEmailService emailService,
            IPdfServiceClient pdfServiceClient)
        {
            _emailService = emailService;
            _pdfServiceClient = pdfServiceClient;
        }

        public async Task Consume(ConsumeContext<SetPasswordEvent> context)
        {
            var message = context.Message;

            var placeholders = new Dictionary<string, string>
            {
                ["@Model.Email"] = message.Email,
                ["@Model.SetPasswordUrl"] = message.Url
            };

            var pdfResponse = await _pdfServiceClient.GeneratePdfAsync(
                SetPasswordTemplateCode,
                placeholders,
                null,
                context.CancellationToken);

            await _emailService.SendEmailWithAttachment(
                message.Email,
                "Set Your Password",
                "Please set your password using the attached PDF.",
                pdfResponse.Pdf.ToByteArray(),
                "SetPassword.pdf");
        }
    }
}