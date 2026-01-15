using AnalyticService.Application.Models.Payments;

namespace AnalyticService.Application.Contracts.Payments;

public interface IPaymentService
{
    Task UpsertAsync(
        long id,
        long walletId,
        long amount,
        long userId,
        PaymentStatus status,
        DateTimeOffset updatedAt,
        CancellationToken cancellationToken);

    Task GetSumByUserAsync(long userId, CancellationToken cancellationToken);
}