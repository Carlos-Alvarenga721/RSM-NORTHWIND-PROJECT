using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NorthwindTraders.Api.Controllers;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Customers;
using NorthwindTraders.Application.UseCases.Customers.GetCustomersLookup;

namespace NorthwindTraders.UnitTests.Api.Controllers;

public sealed class CustomersControllerTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnOkWithCustomerLookupData()
    {
        var customers = new List<CustomerLookupResponse>
        {
            new("ALFKI", "Alfreds Futterkiste", "Maria Anders", "Berlin", "Germany")
        };

        var repository = new Mock<ICustomerRepository>();
        repository
            .Setup(customerRepository => customerRepository.GetCustomersLookupAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        var controller = new CustomersController(new GetCustomersLookupUseCase(repository.Object));

        var result = await controller.GetAsync(CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(customers);
        repository.Verify(
            customerRepository => customerRepository.GetCustomersLookupAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
