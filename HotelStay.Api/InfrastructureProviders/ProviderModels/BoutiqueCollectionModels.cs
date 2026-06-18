namespace HotelStay.Api.InfrastructureProviders.ProviderModels;

internal sealed record BoutiqueCollectionListing
{
    public required Guid HotelId { get; init; }

    public required string HotelName { get; init; }

    public required string City { get; init; }

    public required string Country { get; init; }

    public required string RoomType { get; init; }

    public decimal BaseNightlyRate { get; init; }

    public bool Available { get; init; }

    public int AvailableRooms { get; init; }
}

internal sealed record BoutiqueCollectionBookingReference
{
    public required Guid HotelId { get; init; }

    public required Guid RoomId { get; init; }

    public required string ConfirmationCode { get; init; }
}
