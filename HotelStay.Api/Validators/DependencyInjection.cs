using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace HotelStay.Api.Validators;

public static class DependencyInjection
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<SearchRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<SearchHotelsQueryValidator>();
        services.AddValidatorsFromAssemblyContaining<BookingRequestValidator>();

        return services;
    }
}