using HotelStay.Api.Enums;

namespace HotelStay.Api.ApplicationInterfaces;

public interface IDocumentValidationService
{
    Task<DocumentValidationResult> ValidateAsync(string destination, DocumentType documentType, CancellationToken cancellationToken = default);
}

public sealed record DocumentValidationResult(bool IsValid, string? Message)
{
    public static DocumentValidationResult Valid { get; } = new(true, null);

    public static DocumentValidationResult Invalid(string message) => new(false, message);
}