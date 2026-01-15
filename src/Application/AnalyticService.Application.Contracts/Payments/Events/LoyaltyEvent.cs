using Itmo.Dev.Platform.Events;

namespace AnalyticService.Application.Contracts.Payments.Events;

public record LoyaltyEvent(long UserId, long PaymentsSum, DateTimeOffset PublishedAt) : IEvent;