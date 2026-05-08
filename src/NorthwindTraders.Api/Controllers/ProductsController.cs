using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.UseCases.Products.GetProductsLookup;

namespace NorthwindTraders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(GetProductsLookupUseCase getProductsLookupUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var products = await getProductsLookupUseCase.ExecuteAsync(cancellationToken);

        return Ok(products);
    }
}
