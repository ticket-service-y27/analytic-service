using AnalyticService.Application.Abstractions.Events;
using AnalyticService.Application.Models.Events;
using Npgsql;
using System.Runtime.CompilerServices;

namespace AnalyticService.Infrastructure.DataAccess.Repositories;

public class EventRepository : IEventRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public EventRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<EventModel?> GetEventAsync(long venueId, CancellationToken ct)
    {
        const string sql = """
                           select id, start_date, venue_id, artist_id, total_seats, occupied_seats
                           from events
                           where venue_id = @Id
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@Id", venueId));

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
        {
            return null;
        }

        return new EventModel(
            Id: reader.GetInt64(0),
            StartDate: reader.GetFieldValue<DateTimeOffset>(1),
            VenueId: reader.GetInt64(2),
            ArtistId: reader.GetInt64(3),
            TotalSeats: reader.GetInt64(4),
            OccupiedSeats: reader.GetInt64(5));
    }

    public async Task CreateAsync(
        long id,
        DateTimeOffset startDate,
        long venueId,
        long artistId,
        long totalSeats,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into events (id, start_date, venue_id, artist_id, total_seats, occupied_seats, updated_at)
                           values (@Id, @StartDate, @VenueId, @ArtistId, @TotalSeats, 0, @UpdatedAt)
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        DateTimeOffset time = DateTimeOffset.UtcNow;
        command.Parameters.Add(new NpgsqlParameter("@Now", time));
        command.Parameters.Add(new NpgsqlParameter("@Id", id));
        command.Parameters.Add(new NpgsqlParameter("@StartDate", startDate.ToUniversalTime()));
        command.Parameters.Add(new NpgsqlParameter("@VenueId", venueId));
        command.Parameters.Add(new NpgsqlParameter("@ArtistId", artistId));
        command.Parameters.Add(new NpgsqlParameter("@TotalSeats", totalSeats));
        command.Parameters.Add(new NpgsqlParameter("@UpdatedAt", time));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task UpdateAsync(long seats, long eventId, CancellationToken cancellationToken)
    {
        const string sql = """
                           update events
                           set occupied_seats = @Seats,
                               updated_at = @Now
                           where id = @EventId
                           """;

        DateTimeOffset now = DateTimeOffset.UtcNow;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add(new NpgsqlParameter("@Seats", seats));
        command.Parameters.Add(new NpgsqlParameter("@EventId", eventId));
        command.Parameters.Add(new NpgsqlParameter("@Now", now));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<EventModel> GetTopEventsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken, long? artistId, long? venueId)
    {
        const string sql = """
                           select id, start_date, venue_id, artist_id, total_seats, occupied_seats
                           from events
                           where
                           (@ArtistId is null or artist_id = @ArtistId)
                           and (@VenueId is null or venue_id = @VenueId)
                           order by occupied_seats desc, id asc
                           limit 5
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add(new NpgsqlParameter
        {
            ParameterName = "@ArtistId",
            Value = artistId ?? (object)DBNull.Value,
            DataTypeName = "bigint",
        });
        command.Parameters.Add(new NpgsqlParameter
        {
            ParameterName = "@VenueId",
            Value = venueId ?? (object)DBNull.Value,
            DataTypeName = "bigint",
        });

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new EventModel(
                Id: reader.GetInt64(0),
                StartDate: reader.GetFieldValue<DateTimeOffset>(1),
                VenueId: reader.GetInt64(2),
                ArtistId: reader.GetInt64(3),
                TotalSeats: reader.GetInt64(4),
                OccupiedSeats: reader.GetInt64(5));
        }
    }
}