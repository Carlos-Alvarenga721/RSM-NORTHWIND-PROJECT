using NorthwindTraders.Application.DTOs.Orders;

namespace NorthwindTraders.Application.Abstractions.Persistence;

/// <summary>
/// Application-facing order persistence contract; implementations hide EF Core and Northwind table details.
/// </summary>
public interface IOrderRepository
{
    Task<IReadOnlyList<OrderSummaryResponse>> GetOrdersAsync(CancellationToken cancellationToken = default);

    Task<OrderResponse?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default);

    Task<CreatedOrderReference> AddAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds detail rows after the database-generated OrderId is available.
    /// </summary>
    Task AddDetailsAsync(
        int orderId,
        IReadOnlyList<OrderDetailRequest> details,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(int orderId, UpdateOrderRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int orderId, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int orderId, CancellationToken cancellationToken = default);
}
