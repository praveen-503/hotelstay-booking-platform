using HotelStay.Api.Enums;
using HotelStay.Api.Models;
using HotelStay.Api.Services;
using HotelStay.Api.Validators;

namespace HotelStay.Tests;

public class SearchRequestValidatorTests
{
    [Fact]
    public async Task ValidateAsync_ReturnsError_WhenCityIsMissing()
    {
        var validator = new SearchRequestValidator();
        var request = new SearchRequest
        {
            City = string.Empty,
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            Adults = 2,
            Rooms = 1,
            PreferredRoomType = RoomType.Deluxe
        };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(SearchRequest.City));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsValid_WhenRequestIsWellFormed()
    {
        var validator = new SearchRequestValidator();
        var request = new SearchRequest
        {
            City = "London",
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            Adults = 2,
            Rooms = 1,
            PreferredRoomType = RoomType.Suite
        };

        var result = await validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }
}

public class BookingRequestValidatorTests
{
    [Fact]
    public async Task ValidateAsync_ReturnsError_WhenPassportIsRequiredForInternationalDestination()
    {
        var validator = new BookingRequestValidator(new DocumentValidationService());
        var request = new BookingRequest
        {
            ProviderName = "PremierStays",
            HotelId = Guid.NewGuid(),
            RoomId = Guid.NewGuid(),
            Destination = "London",
            RoomType = RoomType.Deluxe,
            GuestFirstName = "Jane",
            GuestLastName = "Doe",
            DocumentType = DocumentType.NationalId,
            DocumentNumber = "NID998877",
            Email = "jane@example.com",
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            Adults = 2,
            Children = 0
        };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorCode == "DOCUMENT_VALIDATION");
        Assert.Contains(result.Errors, error => error.ErrorMessage == "Passport required for international destinations");
    }

    [Fact]
    public async Task ValidateAsync_ReturnsValid_WhenRequestIsWellFormed()
    {
        var validator = new BookingRequestValidator(new DocumentValidationService());
        var request = new BookingRequest
        {
            ProviderName = "BudgetNests",
            HotelId = Guid.NewGuid(),
            RoomId = Guid.NewGuid(),
            Destination = "Hyderabad",
            RoomType = RoomType.Deluxe,
            GuestFirstName = "Jane",
            GuestLastName = "Doe",
            DocumentType = DocumentType.NationalId,
            DocumentNumber = "NID998877",
            Email = "jane.doe@example.com",
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            Adults = 2,
            Children = 0
        };

        var result = await validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }
}

public class DocumentValidationServiceTests
{
    [Fact]
    public async Task ValidateAsync_ReturnsInvalid_ForInternationalNationalId()
    {
        var service = new DocumentValidationService();

        var result = await service.ValidateAsync("Paris", DocumentType.NationalId);

        Assert.False(result.IsValid);
        Assert.Equal("Passport required for international destinations", result.Message);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsValid_ForDomesticNationalId()
    {
        var service = new DocumentValidationService();

        var result = await service.ValidateAsync("Bangalore", DocumentType.NationalId);

        Assert.True(result.IsValid);
    }
}