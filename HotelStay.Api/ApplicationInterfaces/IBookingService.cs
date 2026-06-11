using HotelStay.Api.Models;

namespace HotelStay.Api.ApplicationInterfaces;

public interface IBookingService
{
    Task<BookingResponse> CreateBookingAsync(BookingRequest request, CancellationToken cancellationToken = default);

    Task<BookingResponse?> GetBookingByReferenceAsync(string reference, CancellationToken cancellationToken = default);
}