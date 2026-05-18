using FluentAssertions;
using Moq;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Orders;

namespace NorthwindTraders.UnitTests.Validators;

public sealed class OrderRequestValidatorTests
{
    [Fact]
    public async Task CreateOrderValidator_ShouldRequireOrderDate()
    {
        var validator = CreateCreateOrderValidator();
        var request = BuildCreateOrderRequest(null, DateTime.Today.AddDays(7), null);

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateOrderRequest.OrderDate) &&
            error.ErrorMessage == "OrderDate is required.");
    }

    [Fact]
    public async Task CreateOrderValidator_ShouldRejectRequiredDateBeforeOrderDate()
    {
        var validator = CreateCreateOrderValidator();
        var orderDate = DateTime.Today;
        var request = BuildCreateOrderRequest(orderDate, orderDate.AddDays(-1), null);

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateOrderRequest.RequiredDate) &&
            error.ErrorMessage == "RequiredDate cannot be before OrderDate.");
    }

    [Fact]
    public async Task CreateOrderValidator_ShouldRejectShippedDateBeforeOrderDate()
    {
        var validator = CreateCreateOrderValidator();
        var orderDate = DateTime.Today;
        var request = BuildCreateOrderRequest(orderDate, orderDate.AddDays(7), orderDate.AddDays(-1));

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateOrderRequest.ShippedDate) &&
            error.ErrorMessage == "ShippedDate cannot be before OrderDate.");
    }

    [Fact]
    public async Task CreateOrderValidator_ShouldAllowShippedDateOnOrAfterOrderDate()
    {
        var validator = CreateCreateOrderValidator();
        var orderDate = DateTime.Today;
        var request = BuildCreateOrderRequest(orderDate, orderDate.AddDays(7), orderDate);

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateOrderValidator_ShouldRejectShippedDateBeforeOrderDate()
    {
        var validator = CreateUpdateOrderValidator();
        var orderDate = DateTime.Today;
        var request = BuildUpdateOrderRequest(orderDate, orderDate.AddDays(7), orderDate.AddDays(-1));

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateOrderRequest.ShippedDate) &&
            error.ErrorMessage == "ShippedDate cannot be before OrderDate.");
    }

    [Fact]
    public async Task UpdateOrderValidator_ShouldAllowShippedDateOnOrAfterOrderDate()
    {
        var validator = CreateUpdateOrderValidator();
        var orderDate = DateTime.Today;
        var request = BuildUpdateOrderRequest(orderDate, orderDate.AddDays(7), orderDate);

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    private static CreateOrderRequestValidator CreateCreateOrderValidator()
    {
        var repositories = CreateRepositories();

        return new CreateOrderRequestValidator(
            repositories.CustomerRepository.Object,
            repositories.EmployeeRepository.Object,
            repositories.ShipperRepository.Object,
            repositories.ProductRepository.Object);
    }

    private static UpdateOrderRequestValidator CreateUpdateOrderValidator()
    {
        var repositories = CreateRepositories();

        return new UpdateOrderRequestValidator(
            repositories.CustomerRepository.Object,
            repositories.EmployeeRepository.Object,
            repositories.ShipperRepository.Object,
            repositories.ProductRepository.Object);
    }

    private static TestRepositories CreateRepositories()
    {
        var customerRepository = new Mock<ICustomerRepository>();
        customerRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var employeeRepository = new Mock<IEmployeeRepository>();
        employeeRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var shipperRepository = new Mock<IShipperRepository>();
        shipperRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var productRepository = new Mock<IProductRepository>();
        productRepository
            .Setup(repository => repository.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        return new TestRepositories(customerRepository, employeeRepository, shipperRepository, productRepository);
    }

    private static CreateOrderRequest BuildCreateOrderRequest(
        DateTime? orderDate,
        DateTime? requiredDate,
        DateTime? shippedDate)
    {
        return new CreateOrderRequest(
            "VINET",
            5,
            orderDate,
            requiredDate,
            shippedDate,
            3,
            32.38m,
            "Vins et alcools Chevalier",
            "59 rue de l'Abbaye",
            "Reims",
            null,
            "51100",
            "France",
            null,
            [new OrderDetailRequest(11, 14m, 1, 0)]);
    }

    private static UpdateOrderRequest BuildUpdateOrderRequest(
        DateTime? orderDate,
        DateTime? requiredDate,
        DateTime? shippedDate)
    {
        return new UpdateOrderRequest(
            "VINET",
            5,
            orderDate,
            requiredDate,
            shippedDate,
            3,
            32.38m,
            "Vins et alcools Chevalier",
            "59 rue de l'Abbaye",
            "Reims",
            null,
            "51100",
            "France",
            null,
            [new OrderDetailRequest(11, 14m, 1, 0)]);
    }

    private sealed record TestRepositories(
        Mock<ICustomerRepository> CustomerRepository,
        Mock<IEmployeeRepository> EmployeeRepository,
        Mock<IShipperRepository> ShipperRepository,
        Mock<IProductRepository> ProductRepository);
}
