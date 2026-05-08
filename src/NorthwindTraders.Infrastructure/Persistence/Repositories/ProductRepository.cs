using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Products;
using NorthwindTraders.Infrastructure.Persistence.DbContext;

namespace NorthwindTraders.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(NorthwindDbContext dbContext) : IProductRepository
{
    public async Task<IReadOnlyList<ProductLookupResponse>> GetProductsLookupAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .OrderBy(product => product.ProductName)
            .Select(product => new ProductLookupResponse(
                product.ProductId,
                product.ProductName,
                product.UnitPrice,
                product.UnitsInStock,
                product.Discontinued,
                product.CategoryId,
                product.Category == null ? null : product.Category.CategoryName))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AnyAsync(product => product.ProductId == productId, cancellationToken);
    }
}
