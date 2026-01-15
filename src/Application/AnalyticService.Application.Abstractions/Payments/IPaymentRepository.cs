using AnalyticService.Application.Models.Payments;

namespace AnalyticService.Application.Abstractions.Payments;

public interface IPaymentRepository
{
    Task UpsertAsync(
        long id,
        long walletId,
        long amount,
        long userId,
        PaymentStatus status,
        DateTimeOffset updatedAt,
        CancellationToken cancellationToken);

    Task<long> GetSumSucceededPaymentsByUserAsync(long userId, CancellationToken cancellationToken);

    Task<long> GetSumRefundedPaymentsByUserAsync(long userId, CancellationToken cancellationToken);
}