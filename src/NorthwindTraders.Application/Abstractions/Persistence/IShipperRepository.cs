using NorthwindTraders.Application.DTOs.Shippers;

namespace NorthwindTraders.Application.Abstractions.Persistence;

public interface IShipperRepository
{
    Task<IReadOnlyList<ShipperLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int shipperId, CancellationToken cancellationToken = default);
}
