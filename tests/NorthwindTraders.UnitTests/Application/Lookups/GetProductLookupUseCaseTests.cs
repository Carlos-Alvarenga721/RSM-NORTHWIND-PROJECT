using FluentAssertions;
using Moq;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Products;
using NorthwindTraders.Application.UseCases.Lookups;

namespace NorthwindTraders.UnitTests.Application.Lookups;

public sealed class GetProductLookupUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnProductLookupData()
    {
        var products = new List<ProductLookupDto>
        {
            new(1, "Chai", 18.00m, 39, false)
        };

        var repository = new Mock<IProductRepository>();
        repository
            .Setup(productRepository => productRepository.GetLookupAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var useCase = new GetProductLookupUseCase(repository.Object);

        var result = await useCase.ExecuteAsync();

        result.Should().BeEquivalentTo(products);
        repository.Verify(
            productRepository => productRepository.GetLookupAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
