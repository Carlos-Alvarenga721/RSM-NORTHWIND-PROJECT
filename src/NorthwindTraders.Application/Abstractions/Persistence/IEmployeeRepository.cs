using NorthwindTraders.Application.DTOs.Employees;

namespace NorthwindTraders.Application.Abstractions.Persistence;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<EmployeeLookupResponse>> GetEmployeesLookupAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int employeeId, CancellationToken cancellationToken = default);
}
