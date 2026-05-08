using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NorthwindTraders.Api.Controllers;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Employees;
using NorthwindTraders.Application.UseCases.Employees.GetEmployeesLookup;

namespace NorthwindTraders.UnitTests.Api.Controllers;

public sealed class EmployeesControllerTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnOkWithEmployeeLookupData()
    {
        var employees = new List<EmployeeLookupResponse>
        {
            new(1, "Nancy Davolio", "Sales Representative", "Seattle", "USA")
        };

        var repository = new Mock<IEmployeeRepository>();
        repository
            .Setup(employeeRepository => employeeRepository.GetEmployeesLookupAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        var controller = new EmployeesController(new GetEmployeesLookupUseCase(repository.Object));

        var result = await controller.GetAsync(CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(employees);
        repository.Verify(
            employeeRepository => employeeRepository.GetEmployeesLookupAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
