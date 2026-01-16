using AnalyticService.Application.Models.Events;

namespace AnalyticService.Application.Abstractions.Events;

public interface IEventRepository
{
    Task<EventModel?> GetEventAsync(long venueId, CancellationToken ct);

    Task CreateAsync(
        long id,
        DateTimeOffset startDate,
        long venueId,
        long artistId,
        long totalSeats,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        long seats,
        long eventId,
        CancellationToken cancellationToken);

    IAsyncEnumerable<EventModel> GetTopEventsAsync(CancellationToken cancellationToken, long? artistId, long? venueId);
}