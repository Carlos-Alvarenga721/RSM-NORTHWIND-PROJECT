using FluentValidation;
using NorthwindTraders.Application.DTOs.AddressValidation;

namespace NorthwindTraders.Application.Validators;

public sealed class AddressValidationRequestValidator : AbstractValidator<AddressValidationRequest>
{
    public AddressValidationRequestValidator()
    {
        RuleFor(request => request.AddressLine)
            .NotEmpty()
            .WithMessage("Address line is required.");

        RuleFor(request => request.Country)
            .NotEmpty()
            .WithMessage("Country is required.");
    }
}
