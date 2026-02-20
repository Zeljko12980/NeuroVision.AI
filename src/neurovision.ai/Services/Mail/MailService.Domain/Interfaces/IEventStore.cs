namespace MailService.Domain.Interfaces
{
    public interface IEventStore
    {
        Task SaveEventAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default);
        Task<List<DomainEvent>> GetEventsForAggregateAsync(Guid aggregateId, CancellationToken cancellationToken = default);
        Task<List<DomainEvent>> GetAllEventsAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
    }
}
