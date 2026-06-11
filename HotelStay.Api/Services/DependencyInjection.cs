using HotelStay.Api.ApplicationInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HotelStay.Api.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDocumentValidationService, DocumentValidationService>();
        services.AddScoped<IHotelSearchService, HotelSearchService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}