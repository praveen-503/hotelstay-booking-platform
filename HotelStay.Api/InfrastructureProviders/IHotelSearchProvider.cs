using HotelStay.Api.Models;

namespace HotelStay.Api.InfrastructureProviders;

public interface IHotelSearchProvider
{
    Task<IReadOnlyList<HotelResult>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default);
}