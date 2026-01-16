using AnalyticService.Application.Contracts.Events;
using EventService.Infrastructure.Messaging.Contracts;
using Itmo.Dev.Platform.Kafka.Consumer;

namespace AnalyticService.Presentation.Kafka.ConsumerHandlers;

public class ReturnedSeatsConsumerHandler : IKafkaConsumerHandler<SeatsReturnedKey, SeatsReturnedValue>
{
    private readonly IEventService _eventService;

    public ReturnedSeatsConsumerHandler(IEventService eventService)
    {
        _eventService = eventService;
    }

    public async ValueTask HandleAsync(IEnumerable<IKafkaConsumerMessage<SeatsReturnedKey, SeatsReturnedValue>> messages, CancellationToken cancellationToken)
    {
        foreach (IKafkaConsumerMessage<SeatsReturnedKey, SeatsReturnedValue> message in messages)
        {
            await _eventService.UpdateAsync(message.Value.HallSchemeId, message.Value.ReturnedSeats, true, cancellationToken);
        }
    }
}