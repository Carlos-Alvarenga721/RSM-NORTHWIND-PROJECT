using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Infrastructure.Persistence.DbContext;

namespace NorthwindTraders.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository(NorthwindDbContext dbContext) : IOrderRepository
{
    public async Task<bool> ExistsAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Orders
            .AnyAsync(order => order.OrderId == orderId, cancellationToken);
    }
}
