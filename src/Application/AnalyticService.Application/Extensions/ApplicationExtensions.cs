using AnalyticService.Application.Contracts.Payments;
using AnalyticService.Application.Payments;
using Microsoft.Extensions.DependencyInjection;

namespace AnalyticService.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}