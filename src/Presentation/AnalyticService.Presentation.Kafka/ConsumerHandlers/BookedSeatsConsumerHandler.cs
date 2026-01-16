using AnalyticService.Application.Contracts.Events;
using EventService.Infrastructure.Messaging.Contracts;
using Itmo.Dev.Platform.Kafka.Consumer;

namespace AnalyticService.Presentation.Kafka.ConsumerHandlers;

public class BookedSeatsConsumerHandler : IKafkaConsumerHandler<SeatsBookedKey, SeatsBookedValue>
{
    private readonly IEventService _eventService;

    public BookedSeatsConsumerHandler(IEventService eventService)
    {
        _eventService = eventService;
    }

    public async ValueTask HandleAsync(IEnumerable<IKafkaConsumerMessage<SeatsBookedKey, SeatsBookedValue>> messages, CancellationToken cancellationToken)
    {
        foreach (IKafkaConsumerMessage<SeatsBookedKey, SeatsBookedValue> message in messages)
        {
            await _eventService.UpdateAsync(message.Value.HallSchemeId, message.Value.BookedSeats, false, cancellationToken);
        }
    }
}