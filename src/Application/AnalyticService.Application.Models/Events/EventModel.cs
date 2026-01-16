namespace AnalyticService.Application.Models.Events;

public record EventModel(
    long Id,
    DateTimeOffset StartDate,
    long VenueId,
    long ArtistId,
    long OccupiedSeats,
    long TotalSeats);