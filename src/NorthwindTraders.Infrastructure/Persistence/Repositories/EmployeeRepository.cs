using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Employees;
using NorthwindTraders.Infrastructure.Persistence.DbContext;

namespace NorthwindTraders.Infrastructure.Persistence.Repositories;

public sealed class EmployeeRepository(NorthwindDbContext dbContext) : IEmployeeRepository
{
    public async Task<IReadOnlyList<EmployeeLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Employees
            .AsNoTracking()
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .Select(employee => new EmployeeLookupDto(
                employee.EmployeeId,
                employee.FirstName + " " + employee.LastName,
                employee.Title,
                employee.City,
                employee.Country))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Employees
            .AnyAsync(employee => employee.EmployeeId == employeeId, cancellationToken);
    }
}
