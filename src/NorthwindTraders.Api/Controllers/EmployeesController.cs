using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.UseCases.Lookups;

namespace NorthwindTraders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EmployeesController(GetEmployeeLookupUseCase getEmployeeLookupUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var employees = await getEmployeeLookupUseCase.ExecuteAsync(cancellationToken);

        return Ok(employees);
    }
}
