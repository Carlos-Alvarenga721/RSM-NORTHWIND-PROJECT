using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.Common.Exceptions;

namespace NorthwindTraders.Application.UseCases.Orders.DeleteOrder;

public sealed class DeleteOrderUseCase(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
{
    public async Task ExecuteAsync(int orderId, CancellationToken cancellationToken = default)
    {
        if (!await orderRepository.ExistsAsync(orderId, cancellationToken))
        {
            throw new NotFoundException($"Order {orderId} was not found.");
        }

        await orderRepository.DeleteAsync(orderId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
