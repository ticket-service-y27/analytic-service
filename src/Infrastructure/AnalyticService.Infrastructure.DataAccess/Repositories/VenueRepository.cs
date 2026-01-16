using AnalyticService.Application.Abstractions.Venues;
using AnalyticService.Application.Models.Venues;
using Npgsql;

namespace AnalyticService.Infrastructure.DataAccess.Repositories;

public class VenueRepository : IVenueRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public VenueRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task CreateAsync(long venueId, long totalSeats, string address, long schemeId, CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into venues (id, total_seats, address, hall_scheme_id)
                           values (@Id, @Seats, @Address, @SchemeId)
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add(new NpgsqlParameter("@Id", venueId));
        command.Parameters.Add(new NpgsqlParameter("@Seats", totalSeats));
        command.Parameters.Add(new NpgsqlParameter("@Address", address));
        command.Parameters.Add(new NpgsqlParameter("@SchemeId", schemeId));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<Venue?> GetBySchemeIdAsync(long schemeId, CancellationToken cancellationToken)
    {
        const string sql = """
                           select id, total_seats, address, hall_scheme_id
                           from venues
                           where hall_scheme_id = @Id
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@Id", schemeId));

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new Venue(
            VenueId: reader.GetInt64(0),
            TotalSeats: reader.GetInt64(1),
            Address: reader.GetString(2),
            SchemeId: reader.GetInt64(3));
    }
}