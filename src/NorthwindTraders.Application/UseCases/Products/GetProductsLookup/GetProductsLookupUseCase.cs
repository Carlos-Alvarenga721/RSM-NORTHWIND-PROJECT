using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Products;

namespace NorthwindTraders.Application.UseCases.Products.GetProductsLookup;

public sealed class GetProductsLookupUseCase(IProductRepository productRepository)
{
    public Task<IReadOnlyList<ProductLookupResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return productRepository.GetProductsLookupAsync(cancellationToken);
    }
}
