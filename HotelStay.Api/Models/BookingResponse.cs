namespace HotelStay.Api.Models;

public sealed record BookingResponse
{
    public required string Reference { get; init; }

    public required string Provider { get; init; }

    public required string PassengerName { get; init; }

    public required string Destination { get; init; }

    public required string RoomType { get; init; }

    public required string DocumentType { get; init; }

    public required string DocumentNumber { get; init; }

    public string? CancellationPolicy { get; init; }

    public required string Status { get; init; }

    public decimal TotalPrice { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}