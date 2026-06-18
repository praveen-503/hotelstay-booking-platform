using HotelStay.Api.Enums;
using HotelStay.Api.InfrastructureProviders;
using HotelStay.Api.Models;

namespace HotelStay.Tests;

public class ProviderArchitectureTests
{
    [Fact]
    public async Task PremierStaysProvider_ReturnsPascalCaseResults_WithPremiumFields()
    {
        IHotelProvider provider = new PremierStaysProvider();

        var results = await provider.SearchAsync(new SearchRequest
        {
            City = "London"
        });

        Assert.NotEmpty(results);
        Assert.All(results, result =>
        {
            Assert.Equal("PremierStays", result.ProviderName);
            Assert.True(result.IsAvailable);
            Assert.True(result.StarRating is 4 or 5);
            Assert.NotNull(result.Amenities);
            Assert.NotEmpty(result.Amenities!);
            Assert.False(string.IsNullOrWhiteSpace(result.CancellationPolicy));
        });
    }

    [Fact]
    public async Task BudgetNestsProvider_ReturnsSnakeCaseResults_WithBudgetFields()
    {
        IHotelProvider provider = new BudgetNestsProvider();

        var results = await provider.SearchAsync(new SearchRequest
        {
            City = "London"
        });

        Assert.NotEmpty(results);
        Assert.Contains(results, result => !result.IsAvailable);
        Assert.All(results, result =>
        {
            Assert.Equal("BudgetNests", result.ProviderName);
            Assert.Null(result.StarRating);
            Assert.True(result.Amenities is null || result.Amenities.Count == 0);
            Assert.True(string.IsNullOrWhiteSpace(result.CancellationPolicy));
        });
    }

    [Fact]
    public async Task HotelSearchService_IgnoresFailingProviders_AndFiltersUnavailableBudgetRooms()
    {
        var service = new HotelStay.Api.Services.HotelSearchService(
            new IHotelProvider[]
            {
                new ThrowingHotelProvider(),
                new PremierStaysProvider(),
                new BudgetNestsProvider()
            },
            new Microsoft.Extensions.Logging.Abstractions.NullLogger<HotelStay.Api.Services.HotelSearchService>());

        var results = await service.SearchAsync(new SearchRequest
        {
            City = "London",
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))
        });

        Assert.Contains(results, result => result.ProviderName == "PremierStays");
        Assert.DoesNotContain(results, result => result.ProviderName == "BudgetNests" && !result.IsAvailable);
        Assert.All(results, result => Assert.True(result.TotalPrice > 0));
    }

    private sealed class ThrowingHotelProvider : IHotelProvider
    {
        public string ProviderName => "ThrowingProvider";

        public Task<List<HotelResult>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Provider failed.");
        }

        public Task BookAsync(BookingRequest request, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task BoutiqueCollectionProvider_ReturnsExpectedResults_WithFeeAndRoomTypeConstraints()
    {
        IHotelProvider provider = new BoutiqueCollectionProvider();

        var results = await provider.SearchAsync(new SearchRequest
        {
            City = "London"
        });

        Assert.NotEmpty(results);
        
        // Assert Deluxe and Suite are available
        var deluxe = results.FirstOrDefault(r => r.RoomType == RoomType.Deluxe);
        Assert.NotNull(deluxe);
        Assert.True(deluxe.IsAvailable);
        Assert.Equal(185.00m, deluxe.NightlyRate); // 170.00m base + 15m boutique fee

        var suite = results.FirstOrDefault(r => r.RoomType == RoomType.Suite);
        Assert.NotNull(suite);
        Assert.True(suite.IsAvailable);
        Assert.Equal(285.00m, suite.NightlyRate); // 270.00m base + 15m boutique fee

        // Assert Standard is unavailable
        var standard = results.FirstOrDefault(r => r.RoomType == RoomType.Standard);
        Assert.NotNull(standard);
        Assert.False(standard.IsAvailable);
        Assert.Equal(0, standard.AvailableRooms);

        Assert.All(results, result =>
        {
            Assert.Equal("BoutiqueCollection", result.ProviderName);
            Assert.Equal("FreeCancellation up to 72h before check-in", result.CancellationPolicy);
        });
    }
}
