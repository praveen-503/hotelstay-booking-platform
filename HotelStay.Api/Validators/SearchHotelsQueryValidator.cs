using FluentValidation;
using HotelStay.Api.Enums;
using HotelStay.Api.Models;

namespace HotelStay.Api.Validators;

public sealed class SearchHotelsQueryValidator : AbstractValidator<SearchHotelsQuery>
{
    public SearchHotelsQueryValidator()
    {
        RuleFor(x => x.Destination)
            .NotEmpty()
            .WithMessage("destination is required.")
            .MaximumLength(100);

        RuleFor(x => x.CheckIn)
            .NotNull()
            .WithMessage("checkIn is required.")
            .Must(value => value.HasValue && value.Value != default)
            .WithMessage("checkIn is required.");

        RuleFor(x => x.CheckOut)
            .NotNull()
            .WithMessage("checkOut is required.")
            .Must(value => value.HasValue && value.Value != default)
            .WithMessage("checkOut is required.");

        RuleFor(x => x)
            .Must(query => query.CheckIn.HasValue && query.CheckOut.HasValue && query.CheckOut.Value > query.CheckIn.Value)
            .WithMessage("checkOut must be after checkIn.");

        RuleFor(x => x.RoomType)
            .Must(type => string.IsNullOrEmpty(type) || Enum.TryParse<RoomType>(type, ignoreCase: true, out _))
            .WithMessage("roomType is invalid. Allowed values: Standard, Deluxe, Suite.");
    }
}