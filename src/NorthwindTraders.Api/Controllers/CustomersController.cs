using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.UseCases.Lookups;

namespace NorthwindTraders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CustomersController(GetCustomerLookupUseCase getCustomerLookupUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var customers = await getCustomerLookupUseCase.ExecuteAsync(cancellationToken);

        return Ok(customers);
    }
}
