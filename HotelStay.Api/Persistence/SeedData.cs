using HotelStay.Api.DomainEntities;
using HotelStay.Api.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelStay.Api.Persistence;

public static class SeedData
{
    public static async Task EnsureSeededAsync(HotelDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Hotels.AnyAsync(cancellationToken))
        {
            dbContext.Hotels.AddRange(CreateHotels());
        }

        if (!await dbContext.Rooms.AnyAsync(cancellationToken))
        {
            dbContext.Rooms.AddRange(CreateRooms());
        }

        if (!await dbContext.Bookings.AnyAsync(cancellationToken))
        {
            dbContext.Bookings.Add(CreateSampleBooking());
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IReadOnlyList<Hotel> CreateHotels()
    {
        return new[]
        {
            new Hotel
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "HotelStay Central",
                City = "London",
                Country = "United Kingdom",
                AddressLine1 = "10 Central Square",
                AddressLine2 = null,
                AverageNightlyRate = 185.00m,
                IsActive = true
            },
            new Hotel
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "HotelStay Banjara",
                City = "Hyderabad",
                Country = "India",
                AddressLine1 = "Road No 1, Banjara Hills",
                AddressLine2 = null,
                AverageNightlyRate = 80.00m,
                IsActive = true
            },
            new Hotel
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "HotelStay Silicon",
                City = "Bangalore",
                Country = "India",
                AddressLine1 = "100 Feet Road, Whitefield",
                AddressLine2 = null,
                AverageNightlyRate = 90.00m,
                IsActive = true
            },
            new Hotel
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "HotelStay Montmartre",
                City = "Paris",
                Country = "France",
                AddressLine1 = "18 Rue de Steinkerque",
                AddressLine2 = null,
                AverageNightlyRate = 150.00m,
                IsActive = true
            },
            new Hotel
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "HotelStay Marina",
                City = "Dubai",
                Country = "UAE",
                AddressLine1 = "Marina Heights, Tower A",
                AddressLine2 = null,
                AverageNightlyRate = 220.00m,
                IsActive = true
            },
            new Hotel
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Name = "HotelStay Piccadilly",
                City = "Manchester",
                Country = "United Kingdom",
                AddressLine1 = "Piccadilly Plaza",
                AddressLine2 = null,
                AverageNightlyRate = 120.00m,
                IsActive = true
            },
            new Hotel
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                Name = "HotelStay Riverside",
                City = "Leeds",
                Country = "United Kingdom",
                AddressLine1 = "Neville Street",
                AddressLine2 = null,
                AverageNightlyRate = 95.00m,
                IsActive = true
            }
        };
    }

    private static IReadOnlyList<Room> CreateRooms()
    {
        return new[]
        {
            // London Rooms
            new Room
            {
                Id = Guid.Parse("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                HotelId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                RoomNumber = "101",
                RoomType = RoomType.Standard,
                NightlyRate = 150.00m,
                IsAvailable = true
            },
            new Room
            {
                Id = Guid.Parse("11111111-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                HotelId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                RoomNumber = "102",
                RoomType = RoomType.Deluxe,
                NightlyRate = 220.00m,
                IsAvailable = true
            },
            // Hyderabad Rooms
            new Room
            {
                Id = Guid.Parse("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                HotelId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                RoomNumber = "201",
                RoomType = RoomType.Standard,
                NightlyRate = 60.00m,
                IsAvailable = true
            },
            new Room
            {
                Id = Guid.Parse("22222222-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                HotelId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                RoomNumber = "202",
                RoomType = RoomType.Suite,
                NightlyRate = 150.00m,
                IsAvailable = true
            },
            // Bangalore Rooms
            new Room
            {
                Id = Guid.Parse("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                HotelId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                RoomNumber = "301",
                RoomType = RoomType.Deluxe,
                NightlyRate = 95.00m,
                IsAvailable = true
            },
            // Paris Rooms
            new Room
            {
                Id = Guid.Parse("44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                HotelId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                RoomNumber = "401",
                RoomType = RoomType.Standard,
                NightlyRate = 110.00m,
                IsAvailable = true
            },
            // Dubai Rooms
            new Room
            {
                Id = Guid.Parse("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                HotelId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                RoomNumber = "501",
                RoomType = RoomType.Suite,
                NightlyRate = 320.00m,
                IsAvailable = true
            },
            // Manchester Rooms
            new Room
            {
                Id = Guid.Parse("66666666-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                HotelId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                RoomNumber = "601",
                RoomType = RoomType.Deluxe,
                NightlyRate = 135.00m,
                IsAvailable = true
            },
            // Leeds Rooms
            new Room
            {
                Id = Guid.Parse("77777777-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                HotelId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                RoomNumber = "701",
                RoomType = RoomType.Standard,
                NightlyRate = 85.00m,
                IsAvailable = true
            }
        };
    }

    private static Booking CreateSampleBooking()
    {
        return new Booking
        {
            Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            Reference = "HSB-PS-20260611-000001",
            Provider = "PremierStays",
            PassengerName = "Ava Patel",
            Destination = "London",
            RoomType = RoomType.Suite.ToString(),
            DocumentType = DocumentType.Passport.ToString(),
            DocumentNumber = "P123456789",
            CancellationPolicy = "Free cancellation up to 24 hours before check-in.",
            TotalPrice = 640.00m,
            Status = "Confirmed",
            CreatedAt = new DateTimeOffset(2026, 6, 11, 12, 0, 0, TimeSpan.Zero)
        };
    }
}
