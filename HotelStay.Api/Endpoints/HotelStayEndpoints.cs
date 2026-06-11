using FluentValidation;
using HotelStay.Api.ApplicationInterfaces;
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
        group.MapPost("/book", CreateBookingAsync)
            .WithName("CreateBooking")
            .Produces<BookingResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("Bookings");

        group.MapGet("/booking/{reference}", GetBookingAsync)
            .WithName("GetBookingByReference")
            .Produces<BookingResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Bookings");

        return group;
    }

    private static async Task<IResult> SearchHotelsAsync(
        SearchHotelsQuery query,
        IValidator<SearchHotelsQuery> validator,
        IHotelSearchService hotelSearchService,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var request = new SearchRequest
        {
            City = query.Destination!,
            CheckInDate = query.CheckIn!.Value,
            CheckOutDate = query.CheckOut!.Value
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
}