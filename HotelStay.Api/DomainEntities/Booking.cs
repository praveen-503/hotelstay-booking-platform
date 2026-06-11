namespace HotelStay.Api.DomainEntities;

public sealed class Booking
{
    public Guid Id { get; set; }

    public required string Reference { get; set; }

    public required string Provider { get; set; }

    public required string PassengerName { get; set; }

    public required string Destination { get; set; }

    public required string RoomType { get; set; }

    public required string DocumentType { get; set; }

    public required string DocumentNumber { get; set; }

    public string? CancellationPolicy { get; set; }

    public decimal TotalPrice { get; set; }

    public required string Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}