using HotelStay.Api.Models;

namespace HotelStay.Api.InfrastructureProviders;

public sealed class HotelSearchProvider : IHotelSearchProvider
{
    private readonly IEnumerable<IHotelProvider> providers;

    public HotelSearchProvider(IEnumerable<IHotelProvider> providers)
    {
        this.providers = providers;
    }

    public Task<IReadOnlyList<HotelResult>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        return SearchAcrossProvidersAsync(request, cancellationToken);
    }

    private async Task<IReadOnlyList<HotelResult>> SearchAcrossProvidersAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        var providerResults = await Task.WhenAll(providers.Select(provider => provider.SearchAsync(request, cancellationToken)));

        return providerResults
            .SelectMany(results => results)
            .OrderBy(result => result.ProviderName)
            .ThenBy(result => result.HotelName)
            .ToList();
    }
}