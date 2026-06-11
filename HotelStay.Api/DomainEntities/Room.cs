using HotelStay.Api.Enums;

namespace HotelStay.Api.DomainEntities;

public sealed class Room
{
    public Guid Id { get; set; }

    public Guid HotelId { get; set; }

    public Hotel Hotel { get; set; } = default!;

    public string RoomNumber { get; set; } = string.Empty;

    public RoomType RoomType { get; set; }

    public decimal NightlyRate { get; set; }

    public bool IsAvailable { get; set; } = true;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}