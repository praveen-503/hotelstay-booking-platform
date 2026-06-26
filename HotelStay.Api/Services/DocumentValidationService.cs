using HotelStay.Api.ApplicationInterfaces;
using HotelStay.Api.Enums;

namespace HotelStay.Api.Services;

public sealed class DocumentValidationService : IDocumentValidationService
{
    private static readonly HashSet<string> DomesticCities = new(StringComparer.OrdinalIgnoreCase)
    {
        "Hyderabad",
        "Bangalore"
    };

    private static readonly HashSet<string> InternationalCities = new(StringComparer.OrdinalIgnoreCase)
    {
        "London",
        "Paris",
        "Dubai",
        "Manchester",
        "Leeds"
    };

    public Task<DocumentValidationResult> ValidateAsync(string destination, DocumentType documentType, CancellationToken cancellationToken = default)
    {
        var isDomestic = DomesticCities.Contains(destination);
        var isInternational = InternationalCities.Contains(destination);

        if (!isDomestic && !isInternational)
        {
            return Task.FromResult(DocumentValidationResult.Invalid("Unknown destination city."));
        }

        if (isInternational && documentType != DocumentType.Passport)
        {
            return Task.FromResult(DocumentValidationResult.Invalid("Passport required for international destinations"));
        }

        return Task.FromResult(DocumentValidationResult.Valid);
    }
}