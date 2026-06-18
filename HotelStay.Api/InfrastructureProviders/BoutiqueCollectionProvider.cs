using HotelStay.Api.Enums;
using HotelStay.Api.InfrastructureProviders.ProviderModels;
using HotelStay.Api.Models;

namespace HotelStay.Api.InfrastructureProviders;

public sealed class BoutiqueCollectionProvider : IHotelProvider
{
    private static readonly IReadOnlyList<BoutiqueCollectionListing> Listings = new[]
    {
        new BoutiqueCollectionListing
        {
            HotelId = Guid.Parse("99999999-9999-9999-9999-999999999999"),
            HotelName = "Boutique Collection Heritage",
            City = "London",
            Country = "United Kingdom",
            RoomType = "Deluxe",
            BaseNightlyRate = 170.00m,
            Available = true,
            AvailableRooms = 5
        },
        new BoutiqueCollectionListing
        {
            HotelId = Guid.Parse("88888888-8888-8888-8888-888888888888"),
            HotelName = "Boutique Collection Heritage",
            City = "London",
            Country = "United Kingdom",
            RoomType = "Suite",
            BaseNightlyRate = 270.00m,
            Available = true,
            AvailableRooms = 3
        },
        new BoutiqueCollectionListing
        {
            HotelId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
            HotelName = "Boutique Collection Heritage",
            City = "London",
            Country = "United Kingdom",
            RoomType = "Standard",
            BaseNightlyRate = 100.00m,
            Available = true,
            AvailableRooms = 1
        },
        new BoutiqueCollectionListing
        {
            HotelId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            HotelName = "Boutique Collection Paris",
            City = "Paris",
            Country = "France",
            RoomType = "Deluxe",
            BaseNightlyRate = 180.00m,
            Available = true,
            AvailableRooms = 4
        },
        new BoutiqueCollectionListing
        {
            HotelId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            HotelName = "Boutique Collection Paris",
            City = "Paris",
            Country = "France",
            RoomType = "Suite",
            BaseNightlyRate = 280.00m,
            Available = true,
            AvailableRooms = 2
        }
    };

    private static readonly IReadOnlyList<BoutiqueCollectionBookingReference> BookingReferences = new[]
    {
        new BoutiqueCollectionBookingReference
        {
            HotelId = Guid.Parse("99999999-9999-9999-9999-999999999999"),
            RoomId = Guid.Parse("99999999-9999-9999-9999-999999999999"),
            ConfirmationCode = "BC-HER-4001"
        }
    };

    public string ProviderName => "BoutiqueCollection";

    public Task<List<HotelResult>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        var results = Listings
            .Where(listing => string.Equals(listing.City, request.City, StringComparison.OrdinalIgnoreCase))
            .Select(Map)
            .ToList();

        return Task.FromResult(results);
    }

    public Task BookAsync(BookingRequest request, CancellationToken cancellationToken = default)
    {
        _ = BookingReferences.FirstOrDefault(reference => reference.HotelId == request.HotelId && reference.RoomId == request.RoomId);
        return Task.CompletedTask;
    }

    private HotelResult Map(BoutiqueCollectionListing listing)
    {
        var isStandard = string.Equals(listing.RoomType, "Standard", StringComparison.OrdinalIgnoreCase);

        return new HotelResult
        {
            HotelId = listing.HotelId,
            ProviderName = ProviderName,
            HotelName = listing.HotelName,
            City = listing.City,
            Country = listing.Country,
            RoomType = Enum.Parse<RoomType>(listing.RoomType, ignoreCase: true),
            NightlyRate = listing.BaseNightlyRate + 15.00m, // base nightly rate + a boutique_fee of £15 per night
            IsAvailable = listing.Available && !isStandard, // Deluxe and Suite only, Standard returns unavailable
            AverageRating = 4.7m,
            StarRating = 4,
            Amenities = new[] { "WiFi", "Design Decor", "Boutique Lounge" },
            CancellationPolicy = "FreeCancellation up to 72h before check-in",
            AvailableRooms = isStandard ? 0 : listing.AvailableRooms
        };
    }
}
