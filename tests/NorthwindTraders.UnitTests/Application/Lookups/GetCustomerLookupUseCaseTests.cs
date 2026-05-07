using FluentAssertions;
using Moq;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Customers;
using NorthwindTraders.Application.UseCases.Lookups;

namespace NorthwindTraders.UnitTests.Application.Lookups;

public sealed class GetCustomerLookupUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnCustomerLookupData()
    {
        var customers = new List<CustomerLookupDto>
        {
            new("ALFKI", "Alfreds Futterkiste", "Maria Anders", "Berlin", null, "Germany")
        };

        var repository = new Mock<ICustomerRepository>();
        repository
            .Setup(customerRepository => customerRepository.GetLookupAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        var useCase = new GetCustomerLookupUseCase(repository.Object);

        var result = await useCase.ExecuteAsync();

        result.Should().BeEquivalentTo(customers);
        repository.Verify(
            customerRepository => customerRepository.GetLookupAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
