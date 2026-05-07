using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.UseCases.Lookups;

namespace NorthwindTraders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(GetProductLookupUseCase getProductLookupUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var products = await getProductLookupUseCase.ExecuteAsync(cancellationToken);

        return Ok(products);
    }
}
