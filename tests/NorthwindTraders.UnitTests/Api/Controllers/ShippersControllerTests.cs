using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NorthwindTraders.Api.Controllers;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Shippers;
using NorthwindTraders.Application.UseCases.Shippers.GetShippersLookup;

namespace NorthwindTraders.UnitTests.Api.Controllers;

public sealed class ShippersControllerTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnOkWithShipperLookupData()
    {
        var shippers = new List<ShipperLookupResponse>
        {
            new(1, "Speedy Express", "(503) 555-9831")
        };

        var repository = new Mock<IShipperRepository>();
        repository
            .Setup(shipperRepository => shipperRepository.GetShippersLookupAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(shippers);

        var controller = new ShippersController(new GetShippersLookupUseCase(repository.Object));

        var result = await controller.GetAsync(CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(shippers);
        repository.Verify(
            shipperRepository => shipperRepository.GetShippersLookupAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
