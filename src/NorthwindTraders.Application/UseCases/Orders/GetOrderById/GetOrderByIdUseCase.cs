using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.Common.Exceptions;
using NorthwindTraders.Application.DTOs.Orders;

namespace NorthwindTraders.Application.UseCases.Orders.GetOrderById;

public sealed class GetOrderByIdUseCase(IOrderRepository orderRepository)
{
    public async Task<OrderResponse> ExecuteAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);

        return order ?? throw new NotFoundException($"Order {orderId} was not found.");
    }
}
