using HotelStay.Api.Models;

namespace HotelStay.Api.InfrastructureProviders;

public sealed class BookingProvider : IBookingProvider
{
    public async Task ExecuteBookingAsync(IHotelProvider provider, BookingRequest request, CancellationToken cancellationToken = default)
    {
        await provider.BookAsync(request, cancellationToken);
    }
}