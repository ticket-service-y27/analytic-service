using AnalyticService.Application.Contracts.Payments.Events;
using Google.Protobuf.WellKnownTypes;
using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.Kafka.Producer;
using Loyalty.Kafka.Contracts;

namespace AnalyticService.Presentation.Kafka.ProducerHandlers;

public class LoyaltyHandler : IEventHandler<LoyaltyEvent>
{
    private readonly IKafkaMessageProducer<PaymentsSumKey, PaymentsSumValue> _messageProducer;

    public LoyaltyHandler(IKafkaMessageProducer<PaymentsSumKey, PaymentsSumValue> messageProducer)
    {
        _messageProducer = messageProducer;
    }

    public async ValueTask HandleAsync(LoyaltyEvent evt, CancellationToken cancellationToken)
    {
        var key = new PaymentsSumKey
        {
            UserId = evt.UserId,
        };

        var value = new PaymentsSumValue
        {
            UserId = evt.UserId,
            PaymentsSum = evt.PaymentsSum,
            PublishedAt = evt.PublishedAt.ToTimestamp(),
        };

        var message = new KafkaProducerMessage<PaymentsSumKey, PaymentsSumValue>(key, value);
        await _messageProducer.ProduceAsync(message, cancellationToken);
    }
}