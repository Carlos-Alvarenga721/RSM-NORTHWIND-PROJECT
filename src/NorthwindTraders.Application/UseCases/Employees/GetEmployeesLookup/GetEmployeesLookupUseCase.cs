using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Employees;

namespace NorthwindTraders.Application.UseCases.Employees.GetEmployeesLookup;

public sealed class GetEmployeesLookupUseCase(IEmployeeRepository employeeRepository)
{
    public Task<IReadOnlyList<EmployeeLookupResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return employeeRepository.GetEmployeesLookupAsync(cancellationToken);
    }
}
