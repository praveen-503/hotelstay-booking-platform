using FluentValidation;
using HotelStay.Api.ApplicationInterfaces;
using HotelStay.Api.Models;
using FluentValidation.Results;
using HotelStay.Api.Enums;

namespace HotelStay.Api.Validators;

public sealed class BookingRequestValidator : AbstractValidator<BookingRequest>
{
    public BookingRequestValidator(IDocumentValidationService documentValidationService)
    {
        RuleFor(x => x.ProviderName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.HotelId)
            .NotEmpty();

        RuleFor(x => x.RoomId)
            .NotEmpty();

        RuleFor(x => x.Destination)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.RoomType)
            .IsInEnum();

        RuleFor(x => x.GuestFirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.GuestLastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DocumentNumber)
            .NotEmpty()
            .Must((request, docNum) =>
            {
                var docNumTrimmed = docNum?.Trim() ?? string.Empty;
                if (request.DocumentType == DocumentType.Passport)
                {
                    return docNumTrimmed.Length is >= 6 and <= 9 && docNumTrimmed.All(char.IsLetterOrDigit);
                }
                if (request.DocumentType == DocumentType.NationalId)
                {
                    return docNumTrimmed.Length is >= 6 and <= 20 && docNumTrimmed.All(char.IsLetterOrDigit);
                }
                return false;
            })
            .WithMessage("Document number must be alphanumeric. Passport: 6-9 characters. National ID: 6-20 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.CheckInDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)))
            .WithMessage("Check-in date cannot be in the past.");

        RuleFor(x => x.CheckOutDate)
            .GreaterThan(x => x.CheckInDate)
            .WithMessage("Check-out date must be after check-in date.");

        RuleFor(x => x.Adults)
            .InclusiveBetween(1, 10);

        RuleFor(x => x.Children)
            .InclusiveBetween(0, 10);

        RuleFor(x => x).CustomAsync(async (request, context, cancellationToken) =>
        {
            var result = await documentValidationService.ValidateAsync(request.Destination, request.DocumentType, cancellationToken);
            if (!result.IsValid && !string.IsNullOrWhiteSpace(result.Message))
            {
                context.AddFailure(new ValidationFailure(nameof(BookingRequest.DocumentType), result.Message)
                {
                    ErrorCode = "DOCUMENT_VALIDATION"
                });
            }
        });
    }
}