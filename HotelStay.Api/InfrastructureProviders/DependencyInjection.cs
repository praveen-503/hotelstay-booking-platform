using Microsoft.Extensions.DependencyInjection;

namespace HotelStay.Api.InfrastructureProviders;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureProviders(this IServiceCollection services)
    {
        services.AddScoped<IHotelProvider, PremierStaysProvider>();
        services.AddScoped<IHotelProvider, BudgetNestsProvider>();
        services.AddScoped<IHotelSearchProvider, HotelSearchProvider>();
        services.AddScoped<IBookingProvider, BookingProvider>();

        return services;
    }
}