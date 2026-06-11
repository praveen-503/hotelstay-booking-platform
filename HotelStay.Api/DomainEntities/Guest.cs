using HotelStay.Api.Enums;

namespace HotelStay.Api.DomainEntities;

public sealed class Guest
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DocumentType DocumentType { get; set; }

    public string DocumentNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}