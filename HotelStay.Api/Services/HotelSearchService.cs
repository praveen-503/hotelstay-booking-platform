using HotelStay.Api.ApplicationInterfaces;
using HotelStay.Api.InfrastructureProviders;
using HotelStay.Api.Models;
using Microsoft.Extensions.Logging;

namespace HotelStay.Api.Services;

public sealed class HotelSearchService : IHotelSearchService
{
    private readonly IEnumerable<IHotelProvider> providers;
    private readonly ILogger<HotelSearchService> logger;

    public HotelSearchService(IEnumerable<IHotelProvider> providers, ILogger<HotelSearchService> logger)
    {
        this.providers = providers;
        this.logger = logger;
    }

    public Task<IReadOnlyList<HotelResult>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        return SearchAsyncInternal(request, cancellationToken);
    }

    private async Task<IReadOnlyList<HotelResult>> SearchAsyncInternal(SearchRequest request, CancellationToken cancellationToken)
    {
        var providerTasks = providers.Select(provider => SearchProviderAsync(provider, request, cancellationToken));
        var providerResults = await Task.WhenAll(providerTasks);

        var results = providerResults
            .SelectMany(result => result)
            .Where(result => !string.Equals(result.ProviderName, "BudgetNests", StringComparison.OrdinalIgnoreCase) || result.IsAvailable)
            .Select(NormalizeResult)
            .Where(result => request.PreferredRoomType == null || result.RoomType == request.PreferredRoomType)
            .Select(ApplyTotalPrice(request))
            .OrderBy(result => result.ProviderName)
            .ThenBy(result => result.HotelName)
            .ToList();

        logger.LogInformation("Hotel search completed for {Destination} with {ResultCount} results from {ProviderCount} providers.", request.City, results.Count, providers.Count());

        return results;
    }

    private async Task<IReadOnlyList<HotelResult>> SearchProviderAsync(IHotelProvider provider, SearchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Querying hotel provider {ProviderName} for {Destination}.", provider.ProviderName, request.City);
            return await provider.SearchAsync(request, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Hotel provider {ProviderName} failed while searching for {Destination}.", provider.ProviderName, request.City);
            return Array.Empty<HotelResult>();
        }
    }

    private static HotelResult NormalizeResult(HotelResult result)
    {
        return result with
        {
            RoomType = NormalizeRoomType(result.RoomType)
        };
    }

    private static Func<HotelResult, HotelResult> ApplyTotalPrice(SearchRequest request)
    {
        var nights = request.CheckOutDate.DayNumber - request.CheckInDate.DayNumber;
        var totalNights = Math.Max(nights, 1);

        return result => result with
        {
            TotalPrice = result.NightlyRate * totalNights
        };
    }

    private static HotelStay.Api.Enums.RoomType NormalizeRoomType(HotelStay.Api.Enums.RoomType roomType)
    {
        return roomType switch
        {
            HotelStay.Api.Enums.RoomType.Standard => HotelStay.Api.Enums.RoomType.Standard,
            HotelStay.Api.Enums.RoomType.Deluxe => HotelStay.Api.Enums.RoomType.Deluxe,
            HotelStay.Api.Enums.RoomType.Suite => HotelStay.Api.Enums.RoomType.Suite,
            _ => HotelStay.Api.Enums.RoomType.Standard
        };
    }
}