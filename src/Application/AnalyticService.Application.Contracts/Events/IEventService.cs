using AnalyticService.Application.Models.Events;

namespace AnalyticService.Application.Contracts.Events;

public interface IEventService
{
    Task CreateAsync(
        long id,
        DateTimeOffset startDate,
        long venueId,
        long artistId,
        long totalSeats,
        CancellationToken ct);

    Task UpdateAsync(
        long schemeId,
        long seats,
        bool returning,
        CancellationToken ct);

    Task<IAsyncEnumerable<EventModel>> GetTopEventsAsync(CancellationToken cancellationToken, long? artistId, long? venueId);
}