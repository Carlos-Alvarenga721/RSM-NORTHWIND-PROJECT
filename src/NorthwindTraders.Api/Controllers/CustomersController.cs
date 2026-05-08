using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.UseCases.Customers.GetCustomersLookup;

namespace NorthwindTraders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CustomersController(GetCustomersLookupUseCase getCustomersLookupUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var customers = await getCustomersLookupUseCase.ExecuteAsync(cancellationToken);

        return Ok(customers);
    }
}
