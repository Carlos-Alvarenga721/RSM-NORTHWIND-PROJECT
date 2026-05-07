using NorthwindTraders.Application.DTOs.Products;

namespace NorthwindTraders.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task<IReadOnlyList<ProductLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int productId, CancellationToken cancellationToken = default);
}
