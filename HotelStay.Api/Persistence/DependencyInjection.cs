using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HotelStay.Api.Persistence.Repositories;

namespace HotelStay.Api.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<HotelDbContext>(options =>
        {
            options.UseInMemoryDatabase("HotelStayDb");
        });

        services.AddScoped<IBookingRepository, BookingRepository>();

        return services;
    }
}
