namespace MailService.API.Handlers
{
    public class ConfirmEmailEventConsumer : IConsumer<ConfirmEmailEvent>
    {
        private const string ConfirmEmailTemplateCode = "EmailConfirmationTemplate";

        private readonly IEmailService _emailService;
        private readonly IPdfServiceClient _pdfServiceClient;

        public ConfirmEmailEventConsumer(
            IEmailService emailService,
            IPdfServiceClient pdfServiceClient)
        {
            _emailService = emailService;
            _pdfServiceClient = pdfServiceClient;
        }

        public async Task Consume(ConsumeContext<ConfirmEmailEvent> context)
        {
            var message = context.Message;

            var placeholders = new Dictionary<string, string>
            {
                ["@Model.FullName"] = message.Email,
                ["@Model.ConfirmationUrl"] = message.token
            };

            var pdfResponse = await _pdfServiceClient.GeneratePdfAsync(
                ConfirmEmailTemplateCode,
                placeholders,
                null,
                context.CancellationToken);

            await _emailService.SendEmailWithAttachment(
                message.Email,
                "Email Confirmation",
                "Please confirm your email using the attached PDF.",
                pdfResponse.Pdf.ToByteArray(),
                "EmailConfirmation.pdf");
        }
    }
}