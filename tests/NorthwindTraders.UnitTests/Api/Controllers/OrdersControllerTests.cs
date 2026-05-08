using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NorthwindTraders.Api.Controllers;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Orders;
using NorthwindTraders.Application.UseCases.Orders.CreateOrder;
using NorthwindTraders.Application.UseCases.Orders.DeleteOrder;
using NorthwindTraders.Application.UseCases.Orders.GetOrderById;
using NorthwindTraders.Application.UseCases.Orders.GetOrders;
using NorthwindTraders.Application.UseCases.Orders.UpdateOrder;

namespace NorthwindTraders.UnitTests.Api.Controllers;

public sealed class OrdersControllerTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnOkWithOrders()
    {
        var orders = new List<OrderSummaryResponse>
        {
            new(10248, "VINET", "Vins et alcools Chevalier", 5, "Steven Buchanan", DateTime.Today,
                DateTime.Today.AddDays(7), null, 3, "Federal Shipping", 32.38m, "Reims", "France", 1, 47.38m)
        };

        var orderRepository = new Mock<IOrderRepository>();
        orderRepository
            .Setup(repository => repository.GetOrdersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        var controller = CreateController(orderRepository.Object);

        var result = await controller.GetAsync(CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(orders);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOkWithOrder()
    {
        var order = CreateOrderResponse();

        var orderRepository = new Mock<IOrderRepository>();
        orderRepository
            .Setup(repository => repository.GetByIdAsync(order.OrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var controller = CreateController(orderRepository.Object);

        var result = await controller.GetByIdAsync(order.OrderId, CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(order);
    }

    private static OrdersController CreateController(IOrderRepository orderRepository)
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var createValidator = new Mock<IValidator<CreateOrderRequest>>();
        var updateValidator = new Mock<IValidator<UpdateOrderRequest>>();

        return new OrdersController(
            new GetOrdersUseCase(orderRepository),
            new GetOrderByIdUseCase(orderRepository),
            new CreateOrderUseCase(orderRepository, unitOfWork.Object, createValidator.Object),
            new UpdateOrderUseCase(orderRepository, unitOfWork.Object, updateValidator.Object),
            new DeleteOrderUseCase(orderRepository, unitOfWork.Object));
    }

    private static OrderResponse CreateOrderResponse()
    {
        return new OrderResponse(
            10248,
            "VINET",
            "Vins et alcools Chevalier",
            5,
            "Steven Buchanan",
            DateTime.Today,
            DateTime.Today.AddDays(7),
            null,
            3,
            "Federal Shipping",
            32.38m,
            "Vins et alcools Chevalier",
            "59 rue de l'Abbaye",
            "Reims",
            null,
            "51100",
            "France",
            47.38m,
            new List<OrderDetailResponse>
            {
                new(11, "Queso Cabrales", 14m, 1, 0, 14m)
            });
    }
}
