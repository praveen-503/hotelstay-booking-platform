using HotelStay.Api.ApplicationInterfaces;
using HotelStay.Api.InfrastructureProviders;
using HotelStay.Api.Models;
using HotelStay.Api.Persistence.Repositories;
using Microsoft.Extensions.Logging;
using HotelStay.Api.Enums;
using HotelStay.Api.DomainEntities;
using FluentValidation;

namespace HotelStay.Api.Services;

public sealed class BookingService : IBookingService
{
    private readonly IEnumerable<IHotelProvider> providers;
    private readonly IBookingProvider bookingProvider;
    private readonly IBookingRepository bookingRepository;
    private readonly IDocumentValidationService documentValidationService;
    private readonly ILogger<BookingService> logger;

    public BookingService(
        IEnumerable<IHotelProvider> providers,
        IBookingProvider bookingProvider,
        IBookingRepository bookingRepository,
        IDocumentValidationService documentValidationService,
        ILogger<BookingService> logger)
    {
        this.providers = providers;
        this.bookingProvider = bookingProvider;
        this.bookingRepository = bookingRepository;
        this.documentValidationService = documentValidationService;
        this.logger = logger;
    }

    public async Task<BookingResponse> CreateBookingAsync(BookingRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateDocumentRulesAsync(request, cancellationToken);
        ValidateDocumentRequirements(request);

        var provider = providers.FirstOrDefault(candidate => string.Equals(candidate.ProviderName, request.ProviderName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Provider '{request.ProviderName}' is not registered.");

        var hotelResult = await FindSelectedHotelAsync(provider, request, cancellationToken);

        await bookingProvider.ExecuteBookingAsync(provider, request, cancellationToken);

        var booking = await CreateAndSaveBookingAsync(request, hotelResult, provider.ProviderName, cancellationToken);

        logger.LogInformation("Booking created successfully with reference {Reference} for provider {Provider}.", booking.Reference, booking.Provider);

        return MapToResponse(booking);
    }

    public async Task<BookingResponse?> GetBookingByReferenceAsync(string reference, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByReferenceAsync(reference, cancellationToken);
        return booking is null ? null : MapToResponse(booking);
    }

    private async Task<Booking> CreateAndSaveBookingAsync(BookingRequest request, HotelResult hotelResult, string providerName, CancellationToken cancellationToken)
    {
        var reference = await GenerateUniqueReferenceAsync(providerName, cancellationToken);
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Reference = reference,
            Provider = providerName,
            PassengerName = $"{request.GuestFirstName} {request.GuestLastName}".Trim(),
            Destination = request.Destination,
            RoomType = NormalizeRoomType(request.RoomType).ToString(),
            DocumentType = request.DocumentType.ToString(),
            DocumentNumber = request.DocumentNumber,
            CancellationPolicy = hotelResult.CancellationPolicy,
            TotalPrice = CalculateTotalPrice(hotelResult.NightlyRate, request.CheckInDate, request.CheckOutDate),
            Status = "Confirmed",
            CreatedAt = DateTimeOffset.UtcNow,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            HotelId = request.HotelId,
            HotelName = hotelResult.HotelName,
            Adults = request.Adults,
            Rooms = 1,
            Currency = GetCurrency(hotelResult.Country)
        };

        await bookingRepository.AddAsync(booking, cancellationToken);
        await bookingRepository.SaveChangesAsync(cancellationToken);

        return booking;
    }

    private async Task<string> GenerateUniqueReferenceAsync(string providerName, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var candidate = $"HSB-{GetProviderCode(providerName)}-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..31].ToUpperInvariant();
            if (!await bookingRepository.ReferenceExistsAsync(candidate, cancellationToken))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Unable to generate a unique booking reference.");
    }

    private async Task<HotelResult> FindSelectedHotelAsync(IHotelProvider provider, BookingRequest request, CancellationToken cancellationToken)
    {
        var hotels = await provider.SearchAsync(new SearchRequest
        {
            City = request.Destination,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            Adults = request.Adults,
            Rooms = 1,
            PreferredRoomType = request.RoomType
        }, cancellationToken);

        var match = hotels.FirstOrDefault(result =>
            result.HotelId == request.HotelId &&
            result.RoomType == NormalizeRoomType(request.RoomType));

        if (match is null)
        {
            throw new InvalidOperationException($"No hotel result was found for provider '{provider.ProviderName}'.");
        }

        if (!match.IsAvailable)
        {
            throw new InvalidOperationException("Selected room is not available.");
        }

        return match;
    }

    private static void ValidateDocumentRequirements(BookingRequest request)
    {
        var documentNumber = request.DocumentNumber.Trim();

        var valid = request.DocumentType switch
        {
            DocumentType.Passport => documentNumber.Length is >= 6 and <= 9 && documentNumber.All(char.IsLetterOrDigit),
            DocumentType.NationalId => documentNumber.Length is >= 6 and <= 20 && documentNumber.All(char.IsLetterOrDigit),
            _ => false
        };

        if (!valid)
        {
            throw new ValidationException($"Document number is invalid for {request.DocumentType}.");
        }
    }

    private async Task ValidateDocumentRulesAsync(BookingRequest request, CancellationToken cancellationToken)
    {
        var result = await documentValidationService.ValidateAsync(request.Destination, request.DocumentType, cancellationToken);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Message ?? "Document validation failed.");
        }
    }

    private static RoomType NormalizeRoomType(RoomType roomType)
    {
        return roomType switch
        {
            RoomType.Standard => RoomType.Standard,
            RoomType.Deluxe => RoomType.Deluxe,
            RoomType.Suite => RoomType.Suite,
            _ => RoomType.Standard
        };
    }

    private static decimal CalculateTotalPrice(decimal nightlyRate, DateOnly checkInDate, DateOnly checkOutDate)
    {
        var nights = Math.Max(checkOutDate.DayNumber - checkInDate.DayNumber, 1);
        return nightlyRate * nights;
    }

    private static string GetProviderCode(string providerName)
    {
        if (string.IsNullOrWhiteSpace(providerName)) return "XX";
        var initials = new string(providerName.Where(char.IsUpper).ToArray());
        return initials.Length >= 2 ? initials[..2] : providerName[..Math.Min(2, providerName.Length)].ToUpperInvariant();
    }

    private static string GetCurrency(string country)
    {
        var c = country?.ToLowerInvariant() ?? "";
        if (c.Contains("united kingdom") || c.Contains("uk") || c.Contains("london")) return "GBP";
        if (c.Contains("india")) return "INR";
        if (c.Contains("france") || c.Contains("spain")) return "EUR";
        if (c.Contains("uae")) return "AED";
        return "USD";
    }

    private static BookingResponse MapToResponse(Booking booking)
    {
        return new BookingResponse
        {
            Reference = booking.Reference,
            Provider = booking.Provider,
            PassengerName = booking.PassengerName,
            Destination = booking.Destination,
            RoomType = booking.RoomType,
            DocumentType = booking.DocumentType,
            DocumentNumber = booking.DocumentNumber,
            CancellationPolicy = booking.CancellationPolicy,
            Status = booking.Status,
            TotalPrice = booking.TotalPrice,
            CreatedAt = booking.CreatedAt,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            HotelId = booking.HotelId,
            HotelName = booking.HotelName,
            Adults = booking.Adults,
            Rooms = booking.Rooms,
            Currency = booking.Currency
        };
    }
}