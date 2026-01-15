using AnalyticService.Application.Abstractions.Payments;
using AnalyticService.Application.Contracts.Payments;
using AnalyticService.Application.Contracts.Payments.Events;
using AnalyticService.Application.Models.Payments;
using Itmo.Dev.Platform.Events;
using System.Transactions;

namespace AnalyticService.Application.Payments;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEventPublisher _eventPublisher;

    public PaymentService(IPaymentRepository paymentRepository, IEventPublisher eventPublisher)
    {
        _paymentRepository = paymentRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task UpsertAsync(
        long id,
        long walletId,
        long amount,
        long userId,
        PaymentStatus status,
        DateTimeOffset updatedAt,
        CancellationToken cancellationToken)
    {
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        await _paymentRepository.UpsertAsync(id, walletId, amount, userId, status, updatedAt, cancellationToken);

        scope.Complete();
    }

    public async Task GetSumByUserAsync(long userId, CancellationToken cancellationToken)
    {
        long sumSucceededPayments = await _paymentRepository.GetSumSucceededPaymentsByUserAsync(userId, cancellationToken);

        long sumRefundedPayments = await _paymentRepository.GetSumRefundedPaymentsByUserAsync(userId, cancellationToken);

        long sum = 0;

        if (sumRefundedPayments < sumSucceededPayments)
        {
            sum = sumSucceededPayments - sumRefundedPayments;
        }

        var evt = new LoyaltyEvent(userId, sum, DateTimeOffset.Now);
        await _eventPublisher.PublishAsync(evt, cancellationToken);
    }
}