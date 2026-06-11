using HotelStay.Api.Enums;
using HotelStay.Api.InfrastructureProviders.ProviderModels;
using HotelStay.Api.Models;

namespace HotelStay.Api.InfrastructureProviders;

public sealed class PremierStaysProvider : IHotelProvider
{
    private static readonly IReadOnlyList<PremierStaysHotelListing> Listings = new[]
    {
        new PremierStaysHotelListing
        {
            HotelId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            HotelName = "PremierStays Royal London",
            City = "London",
            Country = "United Kingdom",
            RoomType = "Suite",
            NightlyRate = 320.00m,
            Available = true,
            StarRating = 5,
            Amenities = new[] { "Spa", "Concierge", "Rooftop Bar" },
            CancellationPolicy = "Free cancellation up to 24 hours before check-in.",
            AvailableRooms = 4
        },
        new PremierStaysHotelListing
        {
            HotelId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            HotelName = "PremierStays City Deluxe",
            City = "Manchester",
            Country = "United Kingdom",
            RoomType = "Deluxe",
            NightlyRate = 245.00m,
            Available = true,
            StarRating = 4,
            Amenities = new[] { "Breakfast", "Gym", "Airport Shuttle" },
            CancellationPolicy = "Free cancellation up to 48 hours before check-in.",
            AvailableRooms = 8
        }
    };

    private static readonly IReadOnlyList<PremierStaysBookingReference> BookingReferences = new[]
    {
        new PremierStaysBookingReference
        {
            HotelId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            RoomId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            ConfirmationCode = "PS-ROYAL-1001"
        },
        new PremierStaysBookingReference
        {
            HotelId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            RoomId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
            ConfirmationCode = "PS-CITY-2001"
        }
    };

    public string ProviderName => "PremierStays";

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

    private HotelResult Map(PremierStaysHotelListing listing)
    {
        return new HotelResult
        {
            HotelId = listing.HotelId,
            ProviderName = ProviderName,
            HotelName = listing.HotelName,
            City = listing.City,
            Country = listing.Country,
            RoomType = Enum.Parse<RoomType>(listing.RoomType, ignoreCase: true),
            NightlyRate = listing.NightlyRate,
            IsAvailable = listing.Available,
            AverageRating = listing.StarRating,
            StarRating = listing.StarRating,
            Amenities = listing.Amenities,
            CancellationPolicy = listing.CancellationPolicy,
            AvailableRooms = listing.AvailableRooms
        };
    }
}