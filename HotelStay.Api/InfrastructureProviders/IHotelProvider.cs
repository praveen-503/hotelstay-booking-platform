using HotelStay.Api.Models;

namespace HotelStay.Api.InfrastructureProviders;

public interface IHotelProvider
{
    string ProviderName { get; }

    Task<List<HotelResult>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default);

    Task BookAsync(BookingRequest request, CancellationToken cancellationToken = default);
}