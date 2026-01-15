namespace AnalyticService.Application.Models.Payments;

public record Payment(
    long Id,
    long WalletId,
    long Amount,
    long UserId,
    PaymentStatus Status,
    DateTimeOffset UpdatedAt);