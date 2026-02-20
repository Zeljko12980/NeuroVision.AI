namespace MailService.Application.Queries
{
    public class GetEmailStatusQuery : IQuery<EmailStatusResponse>
    {
        public Guid EmailId { get; set; }
    }

    public class GetEmailStatusHandler : IQueryHandler<GetEmailStatusQuery, EmailStatusResponse?>
    {
        private readonly IEmailRepository _emailRepository;

        public GetEmailStatusHandler(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<EmailStatusResponse?> Handle(GetEmailStatusQuery request, CancellationToken cancellationToken)
        {
            var email = await _emailRepository.GetByIdAsync(request.EmailId, cancellationToken);

            if (email == null)
                return null;

            return new EmailStatusResponse
            {
                EmailId = email.Id,
                Status = email.Status.ToString(),
                CreatedAt = email.CreatedAt,
                SentAt = email.SentAt,
                FailedAt = email.FailedAt,
                ErrorMessage = email.ErrorMessage,
                RetryCount = email.RetryCount,
                MaxRetries = email.MaxRetries
            };
        }
    }
}
