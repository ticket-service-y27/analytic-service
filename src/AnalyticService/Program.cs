using AnalyticService.Application.Extensions;
using AnalyticService.Infrastructure.DataAccess.Extensions;
using AnalyticService.Infrastructure.DataAccess.Services;
using AnalyticService.Presentation.Kafka.Extensions;
using Itmo.Dev.Platform.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDatabaseOptions(builder.Configuration)
    .AddMigrations()
    .AddNpgsqlDataSource()
    .AddRepositories()
    .AddApplication()
    .AddPresentationKafka(builder.Configuration);

builder.Services.AddPlatformEvents(events => events
    .AddPresentationKafkaEventHandlers());

builder.Services.AddHostedService<MigrationHostedService>();

using IHost host = builder.Build();
await host.RunAsync();