namespace HotelStay.Api.InfrastructureProviders.ProviderModels;

internal sealed record PremierStaysHotelListing
{
    public required Guid HotelId { get; init; }

    public required string HotelName { get; init; }

    public required string City { get; init; }

    public required string Country { get; init; }

    public required string RoomType { get; init; }

    public decimal NightlyRate { get; init; }

    public bool Available { get; init; }

    public int StarRating { get; init; }

    public IReadOnlyList<string> Amenities { get; init; } = Array.Empty<string>();

    public string CancellationPolicy { get; init; } = string.Empty;

    public int AvailableRooms { get; init; }
}

internal sealed record PremierStaysBookingReference
{
    public required Guid HotelId { get; init; }

    public required Guid RoomId { get; init; }

    public required string ConfirmationCode { get; init; }
}