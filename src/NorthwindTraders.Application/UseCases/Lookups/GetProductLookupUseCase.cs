using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Products;

namespace NorthwindTraders.Application.UseCases.Lookups;

public sealed class GetProductLookupUseCase(IProductRepository productRepository)
{
    public Task<IReadOnlyList<ProductLookupDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return productRepository.GetLookupAsync(cancellationToken);
    }
}
