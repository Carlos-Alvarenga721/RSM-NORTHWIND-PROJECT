using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Shippers;
using NorthwindTraders.Infrastructure.Persistence.DbContext;

namespace NorthwindTraders.Infrastructure.Persistence.Repositories;

public sealed class ShipperRepository(NorthwindDbContext dbContext) : IShipperRepository
{
    public async Task<IReadOnlyList<ShipperLookupResponse>> GetShippersLookupAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Shippers
            .AsNoTracking()
            .OrderBy(shipper => shipper.CompanyName)
            .Select(shipper => new ShipperLookupResponse(
                shipper.ShipperId,
                shipper.CompanyName,
                shipper.Phone))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int shipperId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Shippers
            .AnyAsync(shipper => shipper.ShipperId == shipperId, cancellationToken);
    }
}
