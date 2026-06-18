using Microsoft.Extensions.DependencyInjection;

namespace HotelStay.Api.InfrastructureProviders;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureProviders(this IServiceCollection services)
    {
        services.AddScoped<IHotelProvider, PremierStaysProvider>();
        services.AddScoped<IHotelProvider, BudgetNestsProvider>();
        services.AddScoped<IHotelProvider, BoutiqueCollectionProvider>();
        services.AddScoped<IBookingProvider, BookingProvider>();

        return services;
    }
}
