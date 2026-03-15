namespace MailService.API.Handlers
{
    public class UserLoggedInEvent : IConsumer<TwoFactorCodeGeneratedEvent>
    {
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;

        public UserLoggedInEvent(IPdfService pdfService, IEmailService emailService)
        {
            _emailService = emailService;
            _pdfService = pdfService;
        }
        public async Task Consume(ConsumeContext<TwoFactorCodeGeneratedEvent> context)
        {
            var message = context.Message;

            var pdfBytes = await _pdfService.GenerateTwoFactorPdf(message.Email, message.Code);

            await _emailService.SendEmailWithAttachment(
                toEmail: message.Email,
                subject: "Your Two-Factor Authentication Code",
                body: $"For security purposes, your two-factor authentication code has been sent as an attached PDF file. Please open the attachment to retrieve your code and complete your sign-in.",
                attachmentBytes: pdfBytes,
                attachmentFileName: "TwoFactorCode.pdf"
            );
        }
    }
}
