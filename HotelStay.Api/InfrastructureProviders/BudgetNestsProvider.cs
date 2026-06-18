using HotelStay.Api.Enums;
using HotelStay.Api.InfrastructureProviders.ProviderModels;
using HotelStay.Api.Models;

namespace HotelStay.Api.InfrastructureProviders;

public sealed class BudgetNestsProvider : IHotelProvider
{
    private static readonly IReadOnlyList<budget_nests_hotel_listing> Listings = new[]
    {
        new budget_nests_hotel_listing
        {
            hotel_id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            hotel_name = "BudgetNests Central London",
            city = "London",
            country = "United Kingdom",
            room_type = "Standard",
            nightly_rate = 115.00m,
            available = true,
            available_rooms = 12
        },
        new budget_nests_hotel_listing
        {
            hotel_id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            hotel_name = "BudgetNests Dockside",
            city = "London",
            country = "United Kingdom",
            room_type = "Deluxe",
            nightly_rate = 132.00m,
            available = false,
            available_rooms = 0
        },
        new budget_nests_hotel_listing
        {
            hotel_id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
            hotel_name = "BudgetNests Riverside",
            city = "Leeds",
            country = "United Kingdom",
            room_type = "Suite",
            nightly_rate = 148.00m,
            available = true,
            available_rooms = 5
        },
        new budget_nests_hotel_listing
        {
            hotel_id = Guid.Parse("55555555-cccc-dddd-eeee-ffffffffffff"),
            hotel_name = "BudgetNests Gachibowli",
            city = "Hyderabad",
            country = "India",
            room_type = "Standard",
            nightly_rate = 3500.00m,
            available = true,
            available_rooms = 10
        },
        new budget_nests_hotel_listing
        {
            hotel_id = Guid.Parse("66666666-cccc-dddd-eeee-ffffffffffff"),
            hotel_name = "BudgetNests Whitefield",
            city = "Bangalore",
            country = "India",
            room_type = "Standard",
            nightly_rate = 3000.00m,
            available = true,
            available_rooms = 8
        },
        new budget_nests_hotel_listing
        {
            hotel_id = Guid.Parse("77777777-cccc-dddd-eeee-ffffffffffff"),
            hotel_name = "BudgetNests Montmartre",
            city = "Paris",
            country = "France",
            room_type = "Standard",
            nightly_rate = 95.00m,
            available = true,
            available_rooms = 4
        },
        new budget_nests_hotel_listing
        {
            hotel_id = Guid.Parse("88888888-cccc-dddd-eeee-ffffffffffff"),
            hotel_name = "BudgetNests Deira",
            city = "Dubai",
            country = "UAE",
            room_type = "Standard",
            nightly_rate = 110.00m,
            available = true,
            available_rooms = 7
        }
    };

    private static readonly IReadOnlyList<budget_nests_booking_reference> BookingReferences = new[]
    {
        new budget_nests_booking_reference
        {
            hotel_id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            room_id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
            confirmation_code = "BN-LON-3001"
        }
    };

    public string ProviderName => "BudgetNests";

    public Task<List<HotelResult>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        var results = Listings
            .Where(listing => string.Equals(listing.city, request.City, StringComparison.OrdinalIgnoreCase))
            .Select(Map)
            .ToList();

        return Task.FromResult(results);
    }

    public Task BookAsync(BookingRequest request, CancellationToken cancellationToken = default)
    {
        _ = BookingReferences.FirstOrDefault(reference => reference.hotel_id == request.HotelId && reference.room_id == request.RoomId);
        return Task.CompletedTask;
    }

    private HotelResult Map(budget_nests_hotel_listing listing)
    {
        return new HotelResult
        {
            HotelId = listing.hotel_id,
            ProviderName = ProviderName,
            HotelName = listing.hotel_name,
            City = listing.city,
            Country = listing.country,
            RoomType = Enum.Parse<RoomType>(listing.room_type, ignoreCase: true),
            NightlyRate = listing.nightly_rate,
            IsAvailable = listing.available,
            AverageRating = 0m,
            StarRating = null,
            Amenities = Array.Empty<string>(),
            CancellationPolicy = null,
            AvailableRooms = listing.available_rooms
        };
    }
}