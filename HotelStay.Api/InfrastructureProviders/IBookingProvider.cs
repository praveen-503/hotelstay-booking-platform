using HotelStay.Api.Models;

namespace HotelStay.Api.InfrastructureProviders;

public interface IBookingProvider
{
    Task ExecuteBookingAsync(IHotelProvider provider, BookingRequest request, CancellationToken cancellationToken = default);
}