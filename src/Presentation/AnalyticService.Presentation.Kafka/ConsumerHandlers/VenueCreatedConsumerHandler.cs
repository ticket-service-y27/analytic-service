using AnalyticService.Application.Contracts.Venues;
using EventService.Infrastructure.Messaging.Contracts;
using Itmo.Dev.Platform.Kafka.Consumer;

namespace AnalyticService.Presentation.Kafka.ConsumerHandlers;

public class VenueCreatedConsumerHandler : IKafkaConsumerHandler<VenueCreatedKey, VenueCreatedValue>
{
    private readonly IVenueService _venueService;

    public VenueCreatedConsumerHandler(IVenueService venueService)
    {
        _venueService = venueService;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaConsumerMessage<VenueCreatedKey, VenueCreatedValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaConsumerMessage<VenueCreatedKey, VenueCreatedValue> message in messages)
        {
            await _venueService.CreateAsync(
                message.Value.VenueId,
                message.Value.TotalSeats,
                message.Value.Address,
                message.Value.HallSchemeId,
                cancellationToken);
        }
    }
}