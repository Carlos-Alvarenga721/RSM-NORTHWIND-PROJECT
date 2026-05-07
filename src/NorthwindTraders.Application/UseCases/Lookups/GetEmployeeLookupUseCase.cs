using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Employees;

namespace NorthwindTraders.Application.UseCases.Lookups;

public sealed class GetEmployeeLookupUseCase(IEmployeeRepository employeeRepository)
{
    public Task<IReadOnlyList<EmployeeLookupDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return employeeRepository.GetLookupAsync(cancellationToken);
    }
}
