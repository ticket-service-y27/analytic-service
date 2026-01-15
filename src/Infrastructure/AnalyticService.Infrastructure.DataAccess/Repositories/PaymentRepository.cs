using AnalyticService.Application.Abstractions.Payments;
using AnalyticService.Application.Models.Payments;
using Npgsql;

namespace AnalyticService.Infrastructure.DataAccess.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PaymentRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task UpsertAsync(
        long id,
        long walletId,
        long amount,
        long userId,
        PaymentStatus status,
        DateTimeOffset updatedAt,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into payments (payment_id, wallet_id, user_id, status, amount, updated_at)
                           values (@PaymentId, @WalletId, @UserId, @Status, @Amount, @UpdatedAt)
                           on conflict (payment_id) do update set
                               wallet_id  = excluded.wallet_id,
                               user_id    = excluded.user_id,
                               status     = excluded.status,
                               amount     = excluded.amount,
                               updated_at = excluded.updated_at;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        DateTimeOffset upd = DateTimeOffset.UtcNow;
        command.Parameters.Add(new NpgsqlParameter("@PaymentId", id));
        command.Parameters.Add(new NpgsqlParameter("@WalletId", walletId));
        command.Parameters.Add(new NpgsqlParameter("@UserId", userId));
        command.Parameters.Add(new NpgsqlParameter
        {
            ParameterName = "@Status",
            Value = status,
            DataTypeName = "payment_status",
        });
        command.Parameters.Add(new NpgsqlParameter("@Amount", amount));
        command.Parameters.Add(new NpgsqlParameter("@UpdatedAt", upd));
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<long> GetSumSucceededPaymentsByUserAsync(long userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           select coalesce(sum(amount), 0)
                           from payments
                           where user_id = @UserId
                             and status is 'succeeded'
                           """;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@UserId", userId));

        return (long)(await command.ExecuteScalarAsync(cancellationToken) ?? 0);
    }

    public async Task<long> GetSumRefundedPaymentsByUserAsync(long userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           select coalesce(sum(amount), 0)
                           from payments
                           where user_id = @UserId
                             and status is 'refunded'
                           """;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@UserId", userId));

        return (long)(await command.ExecuteScalarAsync(cancellationToken) ?? 0);
    }
}