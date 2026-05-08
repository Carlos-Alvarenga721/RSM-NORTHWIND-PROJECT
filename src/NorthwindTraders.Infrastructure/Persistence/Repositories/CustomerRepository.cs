using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Customers;
using NorthwindTraders.Infrastructure.Persistence.DbContext;

namespace NorthwindTraders.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository(NorthwindDbContext dbContext) : ICustomerRepository
{
    public async Task<IReadOnlyList<CustomerLookupResponse>> GetCustomersLookupAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.CompanyName)
            .Select(customer => new CustomerLookupResponse(
                customer.CustomerId,
                customer.CompanyName,
                customer.ContactName,
                customer.City,
                customer.Country))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string customerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Customers
            .AnyAsync(customer => customer.CustomerId == customerId, cancellationToken);
    }
}
