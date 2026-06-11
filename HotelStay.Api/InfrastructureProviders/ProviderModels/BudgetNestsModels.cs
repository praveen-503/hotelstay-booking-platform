namespace HotelStay.Api.InfrastructureProviders.ProviderModels;

internal sealed record budget_nests_hotel_listing
{
    public required Guid hotel_id { get; init; }

    public required string hotel_name { get; init; }

    public required string city { get; init; }

    public required string country { get; init; }

    public required string room_type { get; init; }

    public decimal nightly_rate { get; init; }

    public bool available { get; init; }

    public int available_rooms { get; init; }
}

internal sealed record budget_nests_booking_reference
{
    public required Guid hotel_id { get; init; }

    public required Guid room_id { get; init; }

    public required string confirmation_code { get; init; }
}