using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.UseCases.Employees.GetEmployeesLookup;

namespace NorthwindTraders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EmployeesController(GetEmployeesLookupUseCase getEmployeesLookupUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var employees = await getEmployeesLookupUseCase.ExecuteAsync(cancellationToken);

        return Ok(employees);
    }
}
