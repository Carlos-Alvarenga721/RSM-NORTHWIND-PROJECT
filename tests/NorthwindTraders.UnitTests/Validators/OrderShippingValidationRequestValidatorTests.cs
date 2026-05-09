using FluentAssertions;
using NorthwindTraders.Application.DTOs.Orders;

namespace NorthwindTraders.UnitTests.Validators;

public sealed class OrderShippingValidationRequestValidatorTests
{
    [Fact]
    public void Validate_ShouldPassForValidatedAddressWithCoordinates()
    {
        var validator = new OrderShippingValidationRequestValidator();
        var request = new OrderShippingValidationRequest(
            "1600 Amphitheatre Pkwy, Mountain View, CA, 94043, US",
            "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
            37.422,
            -122.084,
            "Validated",
            "place-id",
            "Google validated this address at premise level.",
            "PREMISE",
            "PREMISE");

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFailForInvalidStatusAndCoordinates()
    {
        var validator = new OrderShippingValidationRequestValidator();
        var request = new OrderShippingValidationRequest(
            "Address",
            "Address",
            120,
            -220,
            "Invalid",
            null,
            null,
            null,
            null);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(OrderShippingValidationRequest.ValidationStatus));
        result.Errors.Should().Contain(error => error.PropertyName == nameof(OrderShippingValidationRequest.Latitude));
        result.Errors.Should().Contain(error => error.PropertyName == nameof(OrderShippingValidationRequest.Longitude));
    }
}
