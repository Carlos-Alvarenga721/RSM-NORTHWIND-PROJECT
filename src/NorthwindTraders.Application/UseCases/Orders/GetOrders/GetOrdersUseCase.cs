using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Orders;

namespace NorthwindTraders.Application.UseCases.Orders.GetOrders;

public sealed class GetOrdersUseCase(IOrderRepository orderRepository)
{
    public Task<IReadOnlyList<OrderSummaryResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetOrdersAsync(cancellationToken);
    }
}
