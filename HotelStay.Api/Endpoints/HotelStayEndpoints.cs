using FluentValidation;
using HotelStay.Api.ApplicationInterfaces;
using HotelStay.Api.Enums;
using HotelStay.Api.InfrastructureProviders;
using HotelStay.Api.Models;
using Microsoft.AspNetCore.Http;

namespace HotelStay.Api.Endpoints;

public static class HotelStayEndpoints
{
    public static IEndpointRouteBuilder MapHotelStayEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("/api/v1");

        api.MapSearchEndpoints();
        api.MapBookingEndpoints();
        
        api.MapGet("/hotels/{hotelId:guid}", GetHotelByIdAsync)
            .WithName("GetHotelById")
            .Produces<HotelResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Hotels");

        return endpoints;
    }

    private static RouteGroupBuilder MapSearchEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/hotels/search", SearchHotelsAsync)
            .WithName("SearchHotels")
            .Produces<IReadOnlyList<HotelResult>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("Hotels");

        return group;
    }

    private static RouteGroupBuilder MapBookingEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/hotels/book", CreateBookingAsync)
            .WithName("CreateBooking")
            .Produces<BookingResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("Bookings");

        group.MapGet("/hotels/booking/{reference}", GetBookingAsync)
            .WithName("GetBookingByReference")
            .Produces<BookingResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Bookings");

        return group;
    }

    private static async Task<IResult> SearchHotelsAsync(
        [AsParameters] SearchHotelsQuery query,
        IValidator<SearchHotelsQuery> validator,
        IHotelSearchService hotelSearchService,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        RoomType? preferredRoomType = null;
        if (!string.IsNullOrWhiteSpace(query.RoomType) && Enum.TryParse<RoomType>(query.RoomType, ignoreCase: true, out var parsedRoomType))
        {
            preferredRoomType = parsedRoomType;
        }

        var request = new SearchRequest
        {
            City = query.Destination!,
            CheckInDate = query.CheckIn!.Value,
            CheckOutDate = query.CheckOut!.Value,
            PreferredRoomType = preferredRoomType
        };

        var results = await hotelSearchService.SearchAsync(request, cancellationToken);
        return TypedResults.Ok(results);
    }

    private static async Task<IResult> CreateBookingAsync(
        BookingRequest request,
        IValidator<BookingRequest> validator,
        IBookingService bookingService,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var documentValidationError = validationResult.Errors.FirstOrDefault(error => error.ErrorCode == "DOCUMENT_VALIDATION");
            if (documentValidationError is not null)
            {
                return TypedResults.UnprocessableEntity(new { message = documentValidationError.ErrorMessage });
            }

            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var response = await bookingService.CreateBookingAsync(request, cancellationToken);
        return TypedResults.Ok(response);
    }

    private static async Task<IResult> GetBookingAsync(
        string reference,
        IBookingService bookingService,
        CancellationToken cancellationToken)
    {
        var booking = await bookingService.GetBookingByReferenceAsync(reference, cancellationToken);
        return booking is null ? TypedResults.NotFound() : TypedResults.Ok(booking);
    }

    public static async Task<IResult> GetHotelByIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask; // Keep compiler happy for async signature

        var result = PremierStaysProvider.GetHotelById(hotelId);
        if (result != null) return TypedResults.Ok(result);

        result = BudgetNestsProvider.GetHotelById(hotelId);
        if (result != null) return TypedResults.Ok(result);

        result = BoutiqueCollectionProvider.GetHotelById(hotelId);
        if (result != null) return TypedResults.Ok(result);

        return TypedResults.NotFound();
    }
}
