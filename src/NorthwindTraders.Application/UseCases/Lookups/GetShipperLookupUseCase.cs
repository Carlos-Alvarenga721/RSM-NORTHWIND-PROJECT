using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Shippers;

namespace NorthwindTraders.Application.UseCases.Lookups;

public sealed class GetShipperLookupUseCase(IShipperRepository shipperRepository)
{
    public Task<IReadOnlyList<ShipperLookupDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return shipperRepository.GetLookupAsync(cancellationToken);
    }
}
