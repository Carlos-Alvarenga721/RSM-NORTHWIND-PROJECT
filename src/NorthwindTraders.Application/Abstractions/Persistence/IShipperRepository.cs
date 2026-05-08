using NorthwindTraders.Application.DTOs.Shippers;

namespace NorthwindTraders.Application.Abstractions.Persistence;

public interface IShipperRepository
{
    Task<IReadOnlyList<ShipperLookupResponse>> GetShippersLookupAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int shipperId, CancellationToken cancellationToken = default);
}
