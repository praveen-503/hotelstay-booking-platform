using HotelStay.Api.DomainEntities;
using HotelStay.Api.Enums;

namespace HotelStay.Api.Persistence;

public static class SeedData
{
    public static readonly Hotel SampleHotelOne = new()
    {
        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        Name = "HotelStay Central",
        City = "London",
        Country = "United Kingdom",
        AddressLine1 = "10 Central Square",
        AddressLine2 = null,
        AverageNightlyRate = 185.00m,
        IsActive = true
    };

    public static readonly Hotel SampleHotelTwo = new()
    {
        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        Name = "HotelStay Bayfront",
        City = "Barcelona",
        Country = "Spain",
        AddressLine1 = "25 Marina Promenade",
        AddressLine2 = "Suite 4",
        AverageNightlyRate = 240.00m,
        IsActive = true
    };

    public static readonly Room SampleRoomOne = new()
    {
        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
        HotelId = SampleHotelOne.Id,
        RoomNumber = "1001",
        RoomType = RoomType.Standard,
        NightlyRate = 175.00m,
        IsAvailable = true
    };

    public static readonly Room SampleRoomTwo = new()
    {
        Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
        HotelId = SampleHotelOne.Id,
        RoomNumber = "1002",
        RoomType = RoomType.Deluxe,
        NightlyRate = 205.00m,
        IsAvailable = true
    };

    public static readonly Room SampleRoomThree = new()
    {
        Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
        HotelId = SampleHotelTwo.Id,
        RoomNumber = "2001",
        RoomType = RoomType.Suite,
        NightlyRate = 320.00m,
        IsAvailable = true
    };

    public static readonly Booking SampleBooking = new()
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