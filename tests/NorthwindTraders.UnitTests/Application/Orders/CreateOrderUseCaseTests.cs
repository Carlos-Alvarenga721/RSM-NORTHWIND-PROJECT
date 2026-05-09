using FluentAssertions;
using FluentValidation;
using Moq;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Orders;
using NorthwindTraders.Application.UseCases.Orders.CreateOrder;

namespace NorthwindTraders.UnitTests.Application.Orders;

public sealed class CreateOrderUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldValidateAddSaveAndReturnCreatedOrder()
    {
        var request = CreateOrderRequest();
        var createdOrder = CreateOrderResponse();

        var orderRepository = new Mock<IOrderRepository>();
        orderRepository
            .Setup(repository => repository.AddAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreatedOrderReference(() => createdOrder.OrderId));
        orderRepository
            .Setup(repository => repository.AddDetailsAsync(
                createdOrder.OrderId,
                request.Details,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        orderRepository
            .Setup(repository => repository.GetByIdAsync(createdOrder.OrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdOrder);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork
            .Setup(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var validator = new InlineValidator<CreateOrderRequest>();

        var useCase = new CreateOrderUseCase(orderRepository.Object, unitOfWork.Object, validator);

        var result = await useCase.ExecuteAsync(request, CancellationToken.None);

        result.Should().BeEquivalentTo(createdOrder);
        orderRepository.Verify(repository => repository.AddAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        orderRepository.Verify(
            repository => repository.AddDetailsAsync(
                createdOrder.OrderId,
                request.Details,
                It.IsAny<CancellationToken>()),
            Times.Once);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    private static CreateOrderRequest CreateOrderRequest()
    {
        return new CreateOrderRequest(
            "VINET",
            5,
            DateTime.Today,
            DateTime.Today.AddDays(7),
            null,
            3,
            32.38m,
            "Vins et alcools Chevalier",
            "59 rue de l'Abbaye",
            "Reims",
            null,
            "51100",
            "France",
            null,
            new List<OrderDetailRequest>
            {
                new(11, 14m, 1, 0)
            });
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
            null,
            47.38m,
            new List<OrderDetailResponse>
            {
                new(11, "Queso Cabrales", 14m, 1, 0, 14m)
            });
    }
}
