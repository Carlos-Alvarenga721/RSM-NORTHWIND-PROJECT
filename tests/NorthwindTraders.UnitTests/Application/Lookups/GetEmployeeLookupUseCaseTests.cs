using FluentAssertions;
using Moq;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Employees;
using NorthwindTraders.Application.UseCases.Lookups;

namespace NorthwindTraders.UnitTests.Application.Lookups;

public sealed class GetEmployeeLookupUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmployeeLookupData()
    {
        var employees = new List<EmployeeLookupDto>
        {
            new(1, "Nancy Davolio", "Sales Representative", "Seattle", "USA")
        };

        var repository = new Mock<IEmployeeRepository>();
        repository
            .Setup(employeeRepository => employeeRepository.GetLookupAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        var useCase = new GetEmployeeLookupUseCase(repository.Object);

        var result = await useCase.ExecuteAsync();

        result.Should().BeEquivalentTo(employees);
        repository.Verify(
            employeeRepository => employeeRepository.GetLookupAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
