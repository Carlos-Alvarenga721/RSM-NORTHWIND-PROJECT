using NorthwindTraders.Application.DTOs.Employees;

namespace NorthwindTraders.Application.Abstractions.Persistence;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<EmployeeLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int employeeId, CancellationToken cancellationToken = default);
}
