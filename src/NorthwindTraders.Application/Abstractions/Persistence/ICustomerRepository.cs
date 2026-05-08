using NorthwindTraders.Application.DTOs.Customers;

namespace NorthwindTraders.Application.Abstractions.Persistence;

public interface ICustomerRepository
{
    Task<IReadOnlyList<CustomerLookupResponse>> GetCustomersLookupAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string customerId, CancellationToken cancellationToken = default);
}
