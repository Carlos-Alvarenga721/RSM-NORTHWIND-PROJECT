using FluentAssertions;
using Moq;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.Common.Exceptions;
using NorthwindTraders.Application.UseCases.Orders.DeleteOrder;

namespace NorthwindTraders.UnitTests.Application.Orders;

public sealed class DeleteOrderUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenOrderDoesNotExist_ShouldThrowNotFoundException()
    {
        const int orderId = 10248;

        var orderRepository = new Mock<IOrderRepository>();
        orderRepository
            .Setup(repository => repository.ExistsAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var unitOfWork = new Mock<IUnitOfWork>();
        var useCase = new DeleteOrderUseCase(orderRepository.Object, unitOfWork.Object);

        var act = async () => await useCase.ExecuteAsync(orderId, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        orderRepository.Verify(repository => repository.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
