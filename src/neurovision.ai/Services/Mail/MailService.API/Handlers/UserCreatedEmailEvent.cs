namespace MailService.API.Handlers
{
    public class UserCreatedEmailEventHandler : IConsumer<UserCreatedEmailEvent>
    {
        private const string UserCredentialsTemplate = "UserCredentialsTemplate";

        private readonly IPdfServiceClient _pdfServiceClient;
        private readonly IEmailService _emailService;

        public UserCreatedEmailEventHandler(
            IPdfServiceClient pdfServiceClient,
            IEmailService emailService)
        {
            _pdfServiceClient = pdfServiceClient;
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<UserCreatedEmailEvent> context)
        {
            var message = context.Message;

            var placeholders = new Dictionary<string, string>
            {
                ["@Model.FullName"] = message.FullName,
                ["@Model.Email"] = message.Email,
                ["@Model.Username"] = message.Username,
                ["@Model.Password"] = message.Password
            };

            var pdfResponse = await _pdfServiceClient.GeneratePdfAsync(
                UserCredentialsTemplate,
                placeholders,
                null,
                context.CancellationToken);

            await _emailService.SendEmailWithAttachment(
                toEmail: message.Email,
                subject: "Your Account Has Been Created",
                body:
                    $"Hello {message.FullName},\n\n" +
                    "Your account has been successfully created. " +
                    "Your login credentials are attached as a PDF for security purposes.\n\n" +
                    "Please keep this information safe.",
                attachmentBytes: pdfResponse.Pdf.ToByteArray(),
                attachmentFileName: "AccountCredentials.pdf");
        }
    }
}