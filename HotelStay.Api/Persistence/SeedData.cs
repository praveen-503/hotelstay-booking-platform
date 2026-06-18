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
                Name = "HotelStay Bayfront",
                City = "Barcelona",
                Country = "Spain",
                AddressLine1 = "25 Marina Promenade",
                AddressLine2 = "Suite 4",
                AverageNightlyRate = 240.00m,
                IsActive = true
            }
        };
    }

    private static IReadOnlyList<Room> CreateRooms()
    {
        return new[]
        {
            new Room
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                HotelId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                RoomNumber = "1001",
                RoomType = RoomType.Standard,
                NightlyRate = 175.00m,
                IsAvailable = true
            },
            new Room
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                HotelId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                RoomNumber = "1002",
                RoomType = RoomType.Deluxe,
                NightlyRate = 205.00m,
                IsAvailable = true
            },
            new Room
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                HotelId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                RoomNumber = "2001",
                RoomType = RoomType.Suite,
                NightlyRate = 320.00m,
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
