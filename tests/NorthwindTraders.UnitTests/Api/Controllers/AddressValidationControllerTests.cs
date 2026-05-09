using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NorthwindTraders.Api.Controllers;
using NorthwindTraders.Application.Abstractions.Services;
using NorthwindTraders.Application.DTOs.AddressValidation;
using NorthwindTraders.Application.UseCases.AddressValidation.ValidateAddress;
using NorthwindTraders.Application.Validators;

namespace NorthwindTraders.UnitTests.Api.Controllers;

public sealed class AddressValidationControllerTests
{
    [Fact]
    public async Task ValidateAsync_ShouldReturnBadRequestForInvalidRequest()
    {
        var controller = CreateController(Mock.Of<IAddressValidationService>());
        var request = new AddressValidationRequest(null, "City", "CA", "94043", null);

        var result = await controller.ValidateAsync(request, CancellationToken.None);

        var badRequest = result.Should().BeAssignableTo<ObjectResult>().Subject;
        badRequest.StatusCode.Should().Be(400);
        badRequest.Value.Should().BeOfType<ValidationProblemDetails>();
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnOkForValidRequest()
    {
        var response = new AddressValidationResponse(
            "1600 Amphitheatre Pkwy, Mountain View, CA, 94043, US",
            "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
            37.422,
            -122.084,
            "Validated",
            "place-id",
            "Google validated this address at premise level.",
            "PREMISE",
            "PREMISE");
        var service = new Mock<IAddressValidationService>();
        service
            .Setup(validation => validation.ValidateAsync(It.IsAny<AddressValidationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = CreateController(service.Object);
        var request = new AddressValidationRequest("1600 Amphitheatre Pkwy", "Mountain View", "CA", "94043", "US");

        var result = await controller.ValidateAsync(request, CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(response);
    }

    private static AddressValidationController CreateController(IAddressValidationService service)
    {
        var validator = new AddressValidationRequestValidator();
        var useCase = new ValidateAddressUseCase(service, validator);

        return new AddressValidationController(useCase, validator);
    }
}
