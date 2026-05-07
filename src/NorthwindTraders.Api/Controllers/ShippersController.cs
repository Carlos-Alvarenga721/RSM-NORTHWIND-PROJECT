using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.UseCases.Lookups;

namespace NorthwindTraders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ShippersController(GetShipperLookupUseCase getShipperLookupUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var shippers = await getShipperLookupUseCase.ExecuteAsync(cancellationToken);

        return Ok(shippers);
    }
}
