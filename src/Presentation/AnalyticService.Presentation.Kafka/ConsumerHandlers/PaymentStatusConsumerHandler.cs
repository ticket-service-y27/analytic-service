using AnalyticService.Application.Contracts.Payments;
using AnalyticService.Application.Models.Payments;
using Itmo.Dev.Platform.Kafka.Consumer;
using Payments.Kafka.Contracts;

namespace AnalyticService.Presentation.Kafka.ConsumerHandlers;

public class PaymentStatusConsumerHandler : IKafkaConsumerHandler<PaymentStatusKey, PaymentStatusValue>
{
    private readonly IPaymentService _paymentService;

    public PaymentStatusConsumerHandler(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaConsumerMessage<PaymentStatusKey, PaymentStatusValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaConsumerMessage<PaymentStatusKey, PaymentStatusValue> message in messages)
        {
            switch (message.Value.EventCase)
            {
                case PaymentStatusValue.EventOneofCase.PaymentPending:
                    await _paymentService.UpsertAsync(
                        message.Key.PaymentId,
                        message.Value.PaymentPending.WalletId,
                        message.Value.PaymentPending.Amount,
                        message.Value.PaymentPending.UserId,
                        PaymentStatus.Pending,
                        DateTimeOffset.Now,
                        cancellationToken);
                    break;
                case PaymentStatusValue.EventOneofCase.PaymentFailed:
                    await _paymentService.UpsertAsync(
                        message.Key.PaymentId,
                        message.Value.PaymentFailed.WalletId,
                        message.Value.PaymentFailed.Amount,
                        message.Value.PaymentFailed.UserId,
                        PaymentStatus.Failed,
                        DateTimeOffset.Now,
                        cancellationToken);
                    break;
                case PaymentStatusValue.EventOneofCase.PaymentSucceeded:
                    await _paymentService.UpsertAsync(
                        message.Key.PaymentId,
                        message.Value.PaymentSucceeded.WalletId,
                        message.Value.PaymentSucceeded.Amount,
                        message.Value.PaymentSucceeded.UserId,
                        PaymentStatus.Succeeded,
                        DateTimeOffset.Now,
                        cancellationToken);

                    await _paymentService.GetSumByUserAsync(message.Value.PaymentSucceeded.UserId, cancellationToken);
                    break;
                case PaymentStatusValue.EventOneofCase.PaymentRefunded:
                    await _paymentService.UpsertAsync(
                        message.Key.PaymentId,
                        message.Value.PaymentRefunded.WalletId,
                        message.Value.PaymentRefunded.Amount,
                        message.Value.PaymentRefunded.UserId,
                        PaymentStatus.Refunded,
                        DateTimeOffset.Now,
                        cancellationToken);

                    await _paymentService.GetSumByUserAsync(message.Value.PaymentRefunded.UserId, cancellationToken);
                    break;
                default:
                    break;
            }
        }
    }
}