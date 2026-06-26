using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotelStay.Api.ApplicationInterfaces;
using HotelStay.Api.DomainEntities;
using HotelStay.Api.Enums;
using HotelStay.Api.InfrastructureProviders;
using HotelStay.Api.Models;
using HotelStay.Api.Persistence.Repositories;
using HotelStay.Api.Services;
using HotelStay.Api.Validators;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace HotelStay.Tests;

public class HotelStaySuiteTests
{
    // Test Case 1: Provider normalization
    [Fact]
    public async Task SearchAsync_ShouldNormalizeRoomTypesAndApplyTotalPrice_WhenResultsAreReturned()
    {
        // Arrange
        var logger = Substitute.For<ILogger<HotelSearchService>>();
        var provider = Substitute.For<IHotelProvider>();
        provider.ProviderName.Returns("TestProvider");

        var hotelResult = new HotelResult
        {
            HotelId = Guid.NewGuid(),
            HotelName = "Test Hotel",
            City = "London",
            Country = "UK",
            RoomType = RoomType.Deluxe,
            NightlyRate = 150m,
            IsAvailable = true,
            ProviderName = "TestProvider"
        };

        var searchRequest = new SearchRequest
        {
            City = "London",
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4)), // 3 nights
            Adults = 2,
            Rooms = 1,
            PreferredRoomType = RoomType.Deluxe
        };

        provider.SearchAsync(searchRequest, Arg.Any<CancellationToken>())
            .Returns(new List<HotelResult> { hotelResult });

        var searchService = new HotelSearchService(new[] { provider }, logger);

        // Act
        var results = await searchService.SearchAsync(searchRequest);

        // Assert
        results.Should().NotBeEmpty();
        var normalizedResult = results.First();
        normalizedResult.RoomType.Should().Be(RoomType.Deluxe);
        normalizedResult.TotalPrice.Should().Be(450m); // 150m * 3 nights
    }

    // Test Case 2: Universal unavailable room filtering
    [Fact]
    public async Task SearchAsync_ShouldFilterOutUnavailableRoomsFromAllProviders()
    {
        // Arrange
        var logger = Substitute.For<ILogger<HotelSearchService>>();

        var budgetNestsProvider = Substitute.For<IHotelProvider>();
        budgetNestsProvider.ProviderName.Returns("BudgetNests");

        var otherProvider = Substitute.For<IHotelProvider>();
        otherProvider.ProviderName.Returns("OtherProvider");

        var searchRequest = new SearchRequest
        {
            City = "Paris",
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2))
        };

        var budgetAvailable = new HotelResult { ProviderName = "BudgetNests", IsAvailable = true, NightlyRate = 60m, HotelName = "Budget Available", City = "Paris", Country = "France" };
        var budgetUnavailable = new HotelResult { ProviderName = "BudgetNests", IsAvailable = false, NightlyRate = 45m, HotelName = "Budget Unavailable", City = "Paris", Country = "France" };
        var otherUnavailable = new HotelResult { ProviderName = "OtherProvider", IsAvailable = false, NightlyRate = 220m, HotelName = "Other Unavailable", City = "Paris", Country = "France" };

        budgetNestsProvider.SearchAsync(searchRequest, Arg.Any<CancellationToken>())
            .Returns(new List<HotelResult> { budgetAvailable, budgetUnavailable });

        otherProvider.SearchAsync(searchRequest, Arg.Any<CancellationToken>())
            .Returns(new List<HotelResult> { otherUnavailable });

        var searchService = new HotelSearchService(new[] { budgetNestsProvider, otherProvider }, logger);

        // Act
        var results = await searchService.SearchAsync(searchRequest);

        // Assert
        results.Should().HaveCount(1);
        results.Should().Contain(r => r.HotelName == "Budget Available");
        results.Should().NotContain(r => r.HotelName == "Budget Unavailable");
        results.Should().NotContain(r => r.HotelName == "Other Unavailable");
    }

    // Test Case 3: Search validation
    [Fact]
    public async Task SearchRequestValidator_ShouldHaveError_WhenCityIsEmpty()
    {
        // Arrange
        var validator = new SearchRequestValidator();
        var request = new SearchRequest
        {
            City = string.Empty,
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            Adults = 2,
            Rooms = 1
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SearchRequest.City));
    }

    [Fact]
    public async Task SearchRequestValidator_ShouldHaveError_WhenCheckOutDateIsBeforeCheckInDate()
    {
        // Arrange
        var validator = new SearchRequestValidator();
        var request = new SearchRequest
        {
            City = "London",
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Adults = 2,
            Rooms = 1
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Check-out date must be after check-in date"));
    }

    // Test Case 4: Document validation
    [Fact]
    public async Task DocumentValidationService_ShouldRequirePassport_ForInternationalDestinations()
    {
        // Arrange
        var service = new DocumentValidationService();

        // Act
        var result = await service.ValidateAsync("London", DocumentType.NationalId);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Passport required for international destinations");
    }

    [Fact]
    public async Task DocumentValidationService_ShouldBeValid_ForDomesticDestinationsWithNationalId()
    {
        // Arrange
        var service = new DocumentValidationService();

        // Act
        var result = await service.ValidateAsync("Hyderabad", DocumentType.NationalId);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    // Test Case 5: Booking creation
    [Fact]
    public async Task CreateBookingAsync_ShouldCreateAndSaveBooking_WhenRequestIsValid()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var roomId = Guid.NewGuid();

        var provider = Substitute.For<IHotelProvider>();
        provider.ProviderName.Returns("PremierStays");

        var matchingHotel = new HotelResult
        {
            HotelId = hotelId,
            HotelName = "Premier Hotel",
            City = "London",
            Country = "UK",
            RoomType = RoomType.Deluxe,
            IsAvailable = true,
            NightlyRate = 120m,
            CancellationPolicy = "Free cancellation up to 24h",
            ProviderName = "PremierStays"
        };

        provider.SearchAsync(Arg.Any<SearchRequest>(), Arg.Any<CancellationToken>())
            .Returns(new List<HotelResult> { matchingHotel });

        var bookingProvider = Substitute.For<IBookingProvider>();
        var bookingRepository = Substitute.For<IBookingRepository>();

        bookingRepository.ReferenceExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var documentValidationService = Substitute.For<IDocumentValidationService>();
        documentValidationService.ValidateAsync(Arg.Any<string>(), Arg.Any<DocumentType>(), Arg.Any<CancellationToken>())
            .Returns(DocumentValidationResult.Valid);

        var logger = Substitute.For<ILogger<BookingService>>();

        var bookingService = new BookingService(
            new[] { provider },
            bookingProvider,
            bookingRepository,
            documentValidationService,
            logger);

        var request = new BookingRequest
        {
            ProviderName = "PremierStays",
            HotelId = hotelId,
            RoomId = roomId,
            Destination = "London",
            RoomType = RoomType.Deluxe,
            GuestFirstName = "John",
            GuestLastName = "Doe",
            DocumentType = DocumentType.Passport,
            DocumentNumber = "ABC123456",
            Email = "john.doe@example.com",
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)), // 2 nights
            Adults = 2
        };

        // Act
        var response = await bookingService.CreateBookingAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Provider.Should().Be("PremierStays");
        response.PassengerName.Should().Be("John Doe");
        response.TotalPrice.Should().Be(240m); // 120m * 2 nights
        response.Status.Should().Be("Confirmed");
        response.CancellationPolicy.Should().Be("Free cancellation up to 24h");

        await bookingRepository.Received(1).AddAsync(Arg.Any<Booking>(), Arg.Any<CancellationToken>());
        await bookingRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    // Test Case 6: Booking retrieval
    [Fact]
    public async Task GetBookingByReferenceAsync_ShouldReturnBookingResponse_WhenBookingExists()
    {
        // Arrange
        var reference = "HSB-PS-20260611-XYZ";
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Reference = reference,
            Provider = "PremierStays",
            PassengerName = "John Doe",
            Destination = "London",
            RoomType = "Deluxe",
            DocumentType = "Passport",
            DocumentNumber = "ABC123456",
            CancellationPolicy = "Free cancellation",
            TotalPrice = 240m,
            Status = "Confirmed",
            CreatedAt = DateTimeOffset.UtcNow,
            HotelName = "PremierStays Royal London",
            Currency = "GBP"
        };

        var bookingRepository = Substitute.For<IBookingRepository>();
        bookingRepository.GetByReferenceAsync(reference, Arg.Any<CancellationToken>())
            .Returns(booking);

        var bookingService = new BookingService(
            Array.Empty<IHotelProvider>(),
            Substitute.For<IBookingProvider>(),
            bookingRepository,
            Substitute.For<IDocumentValidationService>(),
            Substitute.For<ILogger<BookingService>>());

        // Act
        var response = await bookingService.GetBookingByReferenceAsync(reference);

        // Assert
        response.Should().NotBeNull();
        response!.Reference.Should().Be(reference);
        response.PassengerName.Should().Be("John Doe");
        response.TotalPrice.Should().Be(240m);
    }

    // Test Case 7: Provider failure handling
    [Fact]
    public async Task SearchAsync_ShouldLogAndIgnoreFailingProviders_AndReturnResultsFromSucceedingProviders()
    {
        // Arrange
        var logger = Substitute.For<ILogger<HotelSearchService>>();

        var failingProvider = Substitute.For<IHotelProvider>();
        failingProvider.ProviderName.Returns("FailingProvider");
        failingProvider.SearchAsync(Arg.Any<SearchRequest>(), Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Provider failure"));

        var workingProvider = Substitute.For<IHotelProvider>();
        workingProvider.ProviderName.Returns("WorkingProvider");
        var result = new HotelResult
        {
            HotelId = Guid.NewGuid(),
            HotelName = "Working Hotel",
            City = "London",
            Country = "UK",
            RoomType = RoomType.Standard,
            NightlyRate = 100m,
            IsAvailable = true,
            ProviderName = "WorkingProvider"
        };
        workingProvider.SearchAsync(Arg.Any<SearchRequest>(), Arg.Any<CancellationToken>())
            .Returns(new List<HotelResult> { result });

        var searchRequest = new SearchRequest
        {
            City = "London",
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2))
        };

        var searchService = new HotelSearchService(new[] { failingProvider, workingProvider }, logger);

        // Act
        var results = await searchService.SearchAsync(searchRequest);

        // Assert
        results.Should().HaveCount(1);
        results.First().HotelName.Should().Be("Working Hotel");
    }
}
