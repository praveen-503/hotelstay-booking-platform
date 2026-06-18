namespace HotelStay.Api.Models;

public sealed record SearchHotelsQuery
{
    public string? Destination { get; init; }

    public DateOnly? CheckIn { get; init; }

    public DateOnly? CheckOut { get; init; }

    public string? RoomType { get; init; }
}