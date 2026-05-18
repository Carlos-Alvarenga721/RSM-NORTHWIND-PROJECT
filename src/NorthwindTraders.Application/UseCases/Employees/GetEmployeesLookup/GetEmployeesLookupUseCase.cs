using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Employees;

namespace NorthwindTraders.Application.UseCases.Employees.GetEmployeesLookup;

/// <summary>
/// Returns compact employee data for selectors instead of exposing the full Northwind employee entity.
/// </summary>
public sealed class GetEmployeesLookupUseCase(IEmployeeRepository employeeRepository)
{
    public Task<IReadOnlyList<EmployeeLookupResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return employeeRepository.GetEmployeesLookupAsync(cancellationToken);
    }
}
