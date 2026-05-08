using NorthwindTraders.Application.DTOs.Orders;

namespace NorthwindTraders.Application.Abstractions.Persistence;

public interface IOrderRepository
{
    Task<IReadOnlyList<OrderSummaryResponse>> GetOrdersAsync(CancellationToken cancellationToken = default);

    Task<OrderResponse?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default);

    Task<CreatedOrderReference> AddAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);

    Task UpdateAsync(int orderId, UpdateOrderRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int orderId, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int orderId, CancellationToken cancellationToken = default);
}
