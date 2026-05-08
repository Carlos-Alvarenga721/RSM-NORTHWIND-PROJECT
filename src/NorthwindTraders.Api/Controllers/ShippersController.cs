using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.UseCases.Shippers.GetShippersLookup;

namespace NorthwindTraders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ShippersController(GetShippersLookupUseCase getShippersLookupUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var shippers = await getShippersLookupUseCase.ExecuteAsync(cancellationToken);

        return Ok(shippers);
    }
}
