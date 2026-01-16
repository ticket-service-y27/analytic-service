using AnalyticService.Application.Abstractions.Venues;
using AnalyticService.Application.Contracts.Venues;
using System.Transactions;

namespace AnalyticService.Application.Venues;

public class VenueService : IVenueService
{
    private readonly IVenueRepository _venueRepository;

    public VenueService(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task CreateAsync(long venueId, long totalSeats, string address, long schemeId, CancellationToken cancellationToken)
    {
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        await _venueRepository.CreateAsync(venueId, totalSeats, address, schemeId, cancellationToken);

        scope.Complete();
    }
}