using HotelStay.Api.DomainEntities;
using HotelStay.Api.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelStay.Api.Persistence;

public static class SeedData
{
    public static async Task EnsureSeededAsync(HotelDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Bookings.AnyAsync(cancellationToken))
        {
            dbContext.Bookings.Add(CreateSampleBooking());
        }

        await dbContext.SaveChangesAsync(cancellationToken);
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
            CreatedAt = new DateTimeOffset(2026, 6, 11, 12, 0, 0, TimeSpan.Zero),
            CheckInDate = new DateOnly(2026, 6, 15),
            CheckOutDate = new DateOnly(2026, 6, 17),
            HotelId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            HotelName = "PremierStays Royal London",
            Adults = 2,
            Rooms = 1,
            Currency = "GBP"
        };
    }
}
