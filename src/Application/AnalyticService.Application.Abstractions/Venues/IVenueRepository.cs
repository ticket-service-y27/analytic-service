using AnalyticService.Application.Models.Venues;

namespace AnalyticService.Application.Abstractions.Venues;

public interface IVenueRepository
{
    Task CreateAsync(
        long venueId,
        long totalSeats,
        string address,
        long schemeId,
        CancellationToken cancellationToken);

    Task<Venue?> GetBySchemeIdAsync(long schemeId, CancellationToken cancellationToken);
}