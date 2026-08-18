namespace MailService.API.Handlers
{
    public class UserLoggedInEvent : IConsumer<TwoFactorCodeGeneratedEvent>
    {
        private const string TwoFactorTemplateCode = "SECURITY_CODE";

        private readonly IPdfServiceClient _pdfServiceClient;
        private readonly IEmailService _emailService;

        public UserLoggedInEvent(
            IPdfServiceClient pdfServiceClient,
            IEmailService emailService)
        {
            _pdfServiceClient = pdfServiceClient;
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<TwoFactorCodeGeneratedEvent> context)
        {
            var message = context.Message;

            var placeholders = new Dictionary<string, string>
            {
                ["@Model.FullName"] = message.FullName,
                ["@Model.Code"] = message.Code
            };

            var pdfResponse = await _pdfServiceClient.GeneratePdfAsync(
                TwoFactorTemplateCode,
                placeholders,
                null,
                context.CancellationToken);

            await _emailService.SendEmailWithAttachment(
                toEmail: message.Email,
                subject: "Your Two-Factor Authentication Code",
                body: "For security purposes, your two-factor authentication code has been sent as an attached PDF file. Please open the attachment to retrieve your code and complete your sign-in.",
                attachmentBytes: pdfResponse.Pdf.ToByteArray(),
                attachmentFileName: "TwoFactorCode.pdf");
        }
    }
}