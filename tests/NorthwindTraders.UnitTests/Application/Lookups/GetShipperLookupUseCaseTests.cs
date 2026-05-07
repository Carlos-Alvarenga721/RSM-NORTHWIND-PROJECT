using FluentAssertions;
using Moq;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Shippers;
using NorthwindTraders.Application.UseCases.Lookups;

namespace NorthwindTraders.UnitTests.Application.Lookups;

public sealed class GetShipperLookupUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnShipperLookupData()
    {
        var shippers = new List<ShipperLookupDto>
        {
            new(1, "Speedy Express", "(503) 555-9831")
        };

        var repository = new Mock<IShipperRepository>();
        repository
            .Setup(shipperRepository => shipperRepository.GetLookupAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(shippers);

        var useCase = new GetShipperLookupUseCase(repository.Object);

        var result = await useCase.ExecuteAsync();

        result.Should().BeEquivalentTo(shippers);
        repository.Verify(
            shipperRepository => shipperRepository.GetLookupAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
