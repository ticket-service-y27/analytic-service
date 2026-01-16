namespace AnalyticService.Application.Contracts.Venues;

public interface IVenueService
{
    Task CreateAsync(
        long venueId,
        long totalSeats,
        string address,
        long schemeId,
        CancellationToken cancellationToken);
}