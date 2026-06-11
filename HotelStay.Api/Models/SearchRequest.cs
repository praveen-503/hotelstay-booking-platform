using HotelStay.Api.Enums;

namespace HotelStay.Api.Models;

public sealed record SearchRequest
{
    public required string City { get; init; }

    public DateOnly CheckInDate { get; init; }

    public DateOnly CheckOutDate { get; init; }

    public int Adults { get; init; } = 1;

    public int Rooms { get; init; } = 1;

    public RoomType? PreferredRoomType { get; init; }
}