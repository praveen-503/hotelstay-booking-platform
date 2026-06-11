using HotelStay.Api.Models;

namespace HotelStay.Api.ApplicationInterfaces;

public interface IHotelSearchService
{
    Task<IReadOnlyList<HotelResult>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default);
}