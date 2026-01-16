using AnalyticService.Application.Abstractions.Events;
using AnalyticService.Application.Abstractions.Venues;
using AnalyticService.Application.Contracts.Events;
using AnalyticService.Application.Models.Events;
using AnalyticService.Application.Models.Venues;
using System.Transactions;

namespace AnalyticService.Application.Events;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IVenueRepository _venueRepository;

    public EventService(IEventRepository eventRepository, IVenueRepository venueRepository)
    {
        _eventRepository = eventRepository;
        _venueRepository = venueRepository;
    }

    public async Task CreateAsync(long id, DateTimeOffset startDate, long venueId, long artistId, long totalSeats, CancellationToken ct)
    {
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        await _eventRepository.CreateAsync(id, startDate, venueId, artistId, totalSeats, ct);

        scope.Complete();
    }

    public async Task UpdateAsync(long schemeId, long seats, bool returning, CancellationToken ct)
    {
        Venue? venue = await _venueRepository.GetBySchemeIdAsync(schemeId, ct);
        if (venue == null)
        {
            throw new Exception($"Venue with id {schemeId} not found");
        }

        EventModel? eventModel = await _eventRepository.GetEventAsync(venue.VenueId, ct);

        if (eventModel == null)
        {
            throw new Exception("event not found");
        }

        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        long update = eventModel.OccupiedSeats;
        if (returning)
        {
            update -= seats;
        }
        else
        {
            update += seats;
        }

        await _eventRepository.UpdateAsync(update, eventModel.Id, ct);
        scope.Complete();
    }

    public async Task<IAsyncEnumerable<EventModel>> GetTopEventsAsync(CancellationToken cancellationToken, long? artistId, long? venueId)
    {
        IAsyncEnumerable<EventModel> events = _eventRepository.GetTopEventsAsync(cancellationToken, artistId, venueId);

        if (await events.CountAsync(cancellationToken) == 0)
        {
            throw new EventException("events not found");
        }

        return events;
    }
}