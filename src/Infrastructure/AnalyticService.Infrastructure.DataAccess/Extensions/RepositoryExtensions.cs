using AnalyticService.Application.Abstractions.Payments;
using AnalyticService.Application.Models.Payments;
using AnalyticService.Infrastructure.DataAccess.Migrations;
using AnalyticService.Infrastructure.DataAccess.Options;
using AnalyticService.Infrastructure.DataAccess.Repositories;
using AnalyticService.Infrastructure.DataAccess.Services;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AnalyticService.Infrastructure.DataAccess.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddDatabaseOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseSettings>()
            .Bind(configuration.GetSection("DatabaseSettings"));

        return services;
    }

    public static IServiceCollection AddMigrationHostedService(
        this IServiceCollection services)
    {
        services.AddHostedService<MigrationHostedService>();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        return services;
    }

    public static IServiceCollection AddMigrations(this IServiceCollection services)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(sp =>
                    sp.GetRequiredService<IOptionsMonitor<DatabaseSettings>>().CurrentValue.ConnectionString)
                .ScanIn(typeof(CreatePaymentsTable).Assembly)
                .For.Migrations());

        return services;
    }

    public static IServiceCollection AddNpgsqlDataSource(this IServiceCollection services)
    {
        services
            .AddSingleton(sp =>
            {
                string dbSettings = sp.GetRequiredService<IOptionsMonitor<DatabaseSettings>>()
                    .CurrentValue.ConnectionString;
                var sourceBuilder = new NpgsqlDataSourceBuilder(dbSettings);

                sourceBuilder.MapEnum<PaymentStatus>();

                return sourceBuilder.Build();
            });

        return services;
    }
}