namespace MailService.Domain.Interfaces
{
    public interface ITemplateEngine
    {
        Task<string> RenderAsync(string templateId, object model, CancellationToken cancellationToken = default);
        Task<bool> TemplateExistsAsync(string templateId, CancellationToken cancellationToken = default);
    }
}
