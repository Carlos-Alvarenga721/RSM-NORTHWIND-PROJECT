using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Shippers;

namespace NorthwindTraders.Application.UseCases.Shippers.GetShippersLookup;

public sealed class GetShippersLookupUseCase(IShipperRepository shipperRepository)
{
    public Task<IReadOnlyList<ShipperLookupResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return shipperRepository.GetShippersLookupAsync(cancellationToken);
    }
}
