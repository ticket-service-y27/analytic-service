using AnalyticService.Application.Contracts.Events;
using AnalyticService.Application.Contracts.Payments;
using AnalyticService.Application.Contracts.Venues;
using AnalyticService.Application.Events;
using AnalyticService.Application.Payments;
using AnalyticService.Application.Venues;
using Microsoft.Extensions.DependencyInjection;

namespace AnalyticService.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IVenueService, VenueService>();
        services.AddScoped<IEventService, EventService>();

        return services;
    }
}