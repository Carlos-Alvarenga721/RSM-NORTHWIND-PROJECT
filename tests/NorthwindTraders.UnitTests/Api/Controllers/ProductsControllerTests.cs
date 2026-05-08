using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NorthwindTraders.Api.Controllers;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Products;
using NorthwindTraders.Application.UseCases.Products.GetProductsLookup;

namespace NorthwindTraders.UnitTests.Api.Controllers;

public sealed class ProductsControllerTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnOkWithProductLookupData()
    {
        var products = new List<ProductLookupResponse>
        {
            new(1, "Chai", 18.00m, 39, false, 1, "Beverages")
        };

        var repository = new Mock<IProductRepository>();
        repository
            .Setup(productRepository => productRepository.GetProductsLookupAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var controller = new ProductsController(new GetProductsLookupUseCase(repository.Object));

        var result = await controller.GetAsync(CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(products);
        repository.Verify(
            productRepository => productRepository.GetProductsLookupAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
