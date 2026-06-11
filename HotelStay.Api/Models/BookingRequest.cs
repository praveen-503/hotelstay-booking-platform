using HotelStay.Api.Enums;

namespace HotelStay.Api.Models;

public sealed record BookingRequest
{
    public required string ProviderName { get; init; }

    public required Guid HotelId { get; init; }

    public required Guid RoomId { get; init; }

    public required string Destination { get; init; }

    public required RoomType RoomType { get; init; }

    public required string GuestFirstName { get; init; }

    public required string GuestLastName { get; init; }

    public required DocumentType DocumentType { get; init; }

    public required string DocumentNumber { get; init; }

    public required string Email { get; init; }

    public DateOnly CheckInDate { get; init; }

    public DateOnly CheckOutDate { get; init; }

    public int Adults { get; init; } = 1;

    public int Children { get; init; }
}