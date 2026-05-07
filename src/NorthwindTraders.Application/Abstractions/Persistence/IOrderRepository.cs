namespace NorthwindTraders.Application.Abstractions.Persistence;

public interface IOrderRepository
{
    Task<bool> ExistsAsync(int orderId, CancellationToken cancellationToken = default);
}
