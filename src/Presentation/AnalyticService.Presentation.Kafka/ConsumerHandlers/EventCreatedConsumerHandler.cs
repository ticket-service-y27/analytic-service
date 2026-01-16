using AnalyticService.Application.Contracts.Events;
using EventService.Infrastructure.Messaging.Contracts;
using Itmo.Dev.Platform.Kafka.Consumer;

namespace AnalyticService.Presentation.Kafka.ConsumerHandlers;

public class EventCreatedConsumerHandler : IKafkaConsumerHandler<EventCreatedKey, EventCreatedValue>
{
    private readonly IEventService _eventService;

    public EventCreatedConsumerHandler(IEventService eventService)
    {
        _eventService = eventService;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaConsumerMessage<EventCreatedKey, EventCreatedValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaConsumerMessage<EventCreatedKey, EventCreatedValue> message in messages)
        {
            await _eventService.CreateAsync(
                message.Value.EventId,
                message.Value.EventDate.ToDateTimeOffset(),
                message.Value.VenueId,
                message.Value.ArtistId,
                message.Value.TotalSeats,
                cancellationToken);
        }
    }
}