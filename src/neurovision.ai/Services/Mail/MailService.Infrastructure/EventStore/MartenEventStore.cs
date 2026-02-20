using MailService.Domain.Events;
using MailService.Domain.Interfaces;
using Marten;
using Microsoft.Extensions.Logging;

namespace MailService.Infrastructure.EventStore
{
    public class MartenEventStore : IEventStore
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger<MartenEventStore> _logger;

        public MartenEventStore(IDocumentStore documentStore, ILogger<MartenEventStore> logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        public async Task SaveEventAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var session = _documentStore.LightweightSession();

                var eventWrapper = new StoredEvent
                {
                    Id = domainEvent.EventId,
                    EventType = domainEvent.GetType().Name,
                    EventData = System.Text.Json.JsonSerializer.Serialize(domainEvent),
                    OccurredOn = domainEvent.OccurredOn,
                    AggregateId = GetAggregateId(domainEvent)
                };

                session.Store(eventWrapper);
                await session.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Event sačuvan: {EventType} - {EventId}", eventWrapper.EventType, eventWrapper.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška pri čuvanju eventa: {EventType}", domainEvent.GetType().Name);
                throw;
            }
        }

        public async Task<List<DomainEvent>> GetEventsForAggregateAsync(Guid aggregateId, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var session = _documentStore.QuerySession();

                var storedEvents = await session.Query<StoredEvent>()
                    .Where(e => e.AggregateId == aggregateId)
                    .OrderBy(e => e.OccurredOn)
                    .ToListAsync(cancellationToken);

                return storedEvents.Select(DeserializeEvent).Where(e => e != null).ToList()!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška pri učitavanju evenata za agregat: {AggregateId}", aggregateId);
                throw;
            }
        }

        public async Task<List<DomainEvent>> GetAllEventsAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var session = _documentStore.QuerySession();

                var storedEvents = await session.Query<StoredEvent>()
                    .OrderByDescending(e => e.OccurredOn)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync(cancellationToken);

                return storedEvents.Select(DeserializeEvent).Where(e => e != null).ToList()!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška pri učitavanju svih evenata");
                throw;
            }
        }

        private Guid GetAggregateId(DomainEvent domainEvent)
        {
            return domainEvent switch
            {
                EmailCreatedEvent e => e.EmailId,
                EmailSentEvent e => e.EmailId,
                EmailFailedEvent e => e.EmailId,
                EmailRetryingEvent e => e.EmailId,
                _ => Guid.Empty
            };
        }

        private DomainEvent? DeserializeEvent(StoredEvent storedEvent)
        {
            try
            {
                var eventType = Type.GetType($"MailService.Domain.Events.{storedEvent.EventType}, MailService.Domain");
                if (eventType == null)
                {
                    _logger.LogWarning("Nepoznat event type: {EventType}", storedEvent.EventType);
                    return null;
                }

                return System.Text.Json.JsonSerializer.Deserialize(storedEvent.EventData, eventType) as DomainEvent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška pri deserializaciji eventa: {EventType}", storedEvent.EventType);
                return null;
            }
        }
    }

    public class StoredEvent
    {
        public Guid Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string EventData { get; set; } = string.Empty;
        public DateTime OccurredOn { get; set; }
        public Guid AggregateId { get; set; }
    }
}
