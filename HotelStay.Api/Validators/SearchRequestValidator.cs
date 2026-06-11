using FluentValidation;
using HotelStay.Api.Models;

namespace HotelStay.Api.Validators;

public sealed class SearchRequestValidator : AbstractValidator<SearchRequest>
{
    public SearchRequestValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CheckInDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));

        RuleFor(x => x.CheckOutDate)
            .GreaterThan(x => x.CheckInDate)
            .WithMessage("Check-out date must be after check-in date.");

        RuleFor(x => x.Adults)
            .InclusiveBetween(1, 10);

        RuleFor(x => x.Rooms)
            .InclusiveBetween(1, 10);
    }
}