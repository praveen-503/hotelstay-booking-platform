using HotelStay.Api.Enums;

namespace HotelStay.Api.Models;

public sealed record HotelResult
{
    public Guid HotelId { get; init; }

    public required string ProviderName { get; init; }

    public required string HotelName { get; init; }

    public required string City { get; init; }

    public required string Country { get; init; }

    public RoomType RoomType { get; init; }

    public decimal NightlyRate { get; init; }

    public decimal TotalPrice { get; init; }

    public bool IsAvailable { get; init; }

    public decimal AverageRating { get; init; }

    public int? StarRating { get; init; }

    public IReadOnlyList<string>? Amenities { get; init; }

    public string? CancellationPolicy { get; init; }

    public int AvailableRooms { get; init; }
}